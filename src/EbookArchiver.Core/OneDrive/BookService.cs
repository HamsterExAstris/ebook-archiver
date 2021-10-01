using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.Graph;

namespace EbookArchiver.OneDrive
{
    public class BookService
    {
        private readonly GraphServiceClient _graphClient;

        public BookService(IGraphServiceClientFactory graphServiceClientFactory) => _graphClient = graphServiceClientFactory.GetGraphClientForScopes(GraphConstants.FilesReadWriteAppFolder);

        public async Task InitiializeAccessAsync() => await _graphClient.Me
            .Drive
            .Special
            .AppRoot
            .Request()
            .GetAsync();

        public async Task UpdateAuthorPathAsync(Author author)
        {
            string? folderName = ReplaceFileSystemUnlikedCharacters(author.DisplayName);

            // Create or rename the folder for the author.
            if (author.FolderId == null)
            {
                DriveItem? uploadSession = await _graphClient.Me
                    .Drive
                    .Special
                    .AppRoot
                    .Children
                    .Request()
                    .AddAsync(new DriveItem
                    {
                        Name = folderName,
                        Folder = new Folder()
                    });

                author.FolderId = uploadSession.Id;
            }
            else
            {
                // See if we need to rename it.
                DriveItem? item = await _graphClient.Me
                    .Drive
                    .Items[author.FolderId]
                    .Request()
                    .GetAsync();
                if (item.Name != folderName)
                {
                    item.Name = folderName;
                    await _graphClient.Me
                        .Drive
                        .Items[item.Id]
                        .Request()
                        .UpdateAsync(item);
                }
            }
        }

        public async Task UpdateBookPathAsync(Book book, bool createFolder = false)
        {
            // Create or update the folder for the author.
            if (book.Author == null)
            {
                throw new ArgumentException("Author navigation property must be populated.", nameof(book));
            }
            await UpdateAuthorPathAsync(book.Author);

            string? folderName = ReplaceFileSystemUnlikedCharacters(book.FolderName);

            // Create or update the folder for the book.
            if (book.FolderId == null)
            {
                if (createFolder)
                {
                    DriveItem? uploadSession = await _graphClient.Me
                        .Drive
                        .Items[book.Author.FolderId]
                        .Children
                        .Request()
                        .AddAsync(new DriveItem
                        {
                            Name = folderName,
                            Folder = new Folder()
                        });

                    book.FolderId = uploadSession.Id;
                }
            }
            else
            {
                // See if we need to rename it or reparent it.
                DriveItem? item = await _graphClient.Me
                    .Drive
                    .Items[book.FolderId]
                    .Request()
                    .GetAsync();
                bool anyChanges = false;
                if (item.Name != folderName)
                {
                    item.Name = folderName;
                    anyChanges = true;
                }
                if (item.ParentReference.Id != book.Author.FolderId)
                {
                    item.ParentReference.Id = book.Author.FolderId;
                    anyChanges = true;
                }
                if (anyChanges)
                {
                    await _graphClient.Me
                        .Drive
                        .Items[item.Id]
                        .Request()
                        .UpdateAsync(item);
                }
            }
        }

        public async Task UploadEbookAsync(Ebook ebook, Stream? original, Stream? drmFree)
        {
            if (ebook.Book == null)
            {
                throw new ArgumentException("Book property must be populated.", nameof(ebook));
            }

            await UpdateBookPathAsync(ebook.Book, true);

            if (original != null)
            {
                ebook.EbookFileId = await PutFileAsync(ebook.Book.FolderId, ebook.FileName, original);
            }

            if (drmFree != null)
            {
                string? fileName = (ebook.DrmStrippedFileName == ebook.FileName)
                    ? Path.GetFileNameWithoutExtension(ebook.DrmStrippedFileName)
                        + "_dedrm"
                        + Path.GetExtension(ebook.DrmStrippedFileName)
                    : ebook.DrmStrippedFileName;

                ebook.DrmStrippedFileId = await PutFileAsync(ebook.Book.FolderId, fileName, drmFree);
            }
        }

        private async Task<string> PutFileAsync(string? folderId, string? fileName, Stream fileContents)
        {
            // Use properties to specify the conflict behavior
            // in this case, replace
            var uploadProps = new DriveItemUploadableProperties
            {
                ODataType = null,
                AdditionalData = new Dictionary<string, object>
                {
                    { "@microsoft.graph.conflictBehavior", "replace" }
                }
            };

            // POST /me/drive/items/folderId:/fileName:/createUploadSession
            UploadSession? uploadSession = await _graphClient.Me
                .Drive
                .Items[folderId]
                .ItemWithPath(fileName)
                .CreateUploadSession(uploadProps)
                .Request()
                .PostAsync();

            // Max slice size must be a multiple of 320 KiB
            // This amount of bytes will be uploaded each iteration
            int maxSliceSize = 320 * 1024;
            var fileUploadTask =
                new LargeFileUploadTask<DriveItem>(uploadSession, fileContents, maxSliceSize);

            // Start the upload
            UploadResult<DriveItem>? uploadResult = await fileUploadTask.UploadAsync();

            return uploadResult.UploadSucceeded
                ? uploadResult.ItemResponse.Id
                : throw new Exception("Error uploading file.");
        }

        /// <summary>
        /// Handle characters not liked by host file systems (breaks Dropbox
        /// sync with desktop app).
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ReplaceFileSystemUnlikedCharacters(string input)
        {
            // Windows: Directory name cannot end with a period.
            if (input.EndsWith("."))
            {
                input += "_";
            }

            // Windows: Directory name cannot contain a colon.
            return input.Replace(":", "-");
        }

        public Task<Stream> DownloadEbookAsync(string fileId) => _graphClient.Me
            .Drive
            .Items[fileId]
            .Content
            .Request()
            .GetAsync();
    }
}
