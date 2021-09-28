using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace EbookArchiver.OneDrive
{
    public class OneDriveService
    {
        protected readonly ITokenAcquisition _tokenAcquisition;

        public OneDriveService(
            ITokenAcquisition tokenAcquisition) => _tokenAcquisition = tokenAcquisition;

        // Gets a Graph client configured with
        // the specified scopes
        public GraphServiceClient GetGraphClientForScopes(params string[] scopes) =>
            GetAuthenticatedGraphClient(async () =>
                {
                    string? token = await _tokenAcquisition
                        .GetAccessTokenForUserAsync(scopes);

                    // Uncomment to print access token to debug console
                    // This will happen for every Graph call, so leave this
                    // out unless you're debugging your token
                    //_logger.LogInformation($"Access token: {token}");

                    return token;
                }
            );

        // If the Graph client is unable to get a token for the
        // requested scopes, it throws this type of exception.
        public static void InvokeAuthIfNeeded(ServiceException serviceException)
        {
            // Check if this failed because interactive auth is needed
            if (serviceException.InnerException is MicrosoftIdentityWebChallengeUserException)
            {
                // Throwing the exception causes Microsoft.Identity.Web to
                // take over, handling auth (based on scopes defined in the
                // AuthorizeForScopes attribute)
                throw serviceException;
            }
        }

        // Uses a page iterator to get all objects in a collection
        public static async Task<List<T>> GetAllPages<T>(
            GraphServiceClient graphClient,
            ICollectionPage<T> page)
        {
            var allItems = new List<T>();

            var pageIterator = PageIterator<T>.CreatePageIterator(
                graphClient, page,
                (item) =>
                {
                    // This code executes for each item in the
                    // collection
                    allItems.Add(item);
                    return true;
                }
            );

            await pageIterator.IterateAsync();

            return allItems;
        }

        // Uses a page iterator to get all directoryObjects
        // in a collection and cast them to a specific type
        // Will exclude any objects that cannot be case to the
        // requested type
        public static async Task<List<T>> GetAllPagesAsType<T>(
            GraphServiceClient graphClient,
            ICollectionPage<DirectoryObject> page) where T : class
        {
            var allItems = new List<T>();

            var pageIterator = PageIterator<DirectoryObject>.CreatePageIterator(
                graphClient, page,
                (item) =>
                {
                    // This code executes for each item in the
                    // collection
                    if (item is T t)
                    {
                        // Only add if the item is the requested type
                        allItems.Add(t);
                    }

                    return true;
                }
            );

            await pageIterator.IterateAsync();

            return allItems;
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(
            Func<Task<string>> acquireAccessToken) => new(
                new CustomAuthenticationProvider(acquireAccessToken)
            );

        private class CustomAuthenticationProvider : IAuthenticationProvider
        {
            private readonly Func<Task<string>> _acquireAccessToken;
            public CustomAuthenticationProvider(Func<Task<string>> acquireAccessToken) => _acquireAccessToken = acquireAccessToken;

            public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
            {
                string? accessToken = await _acquireAccessToken.Invoke();

                // Add the token in the Authorization header
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer", accessToken
                );
            }
        }

        private readonly string[] _filesScopes =
            new[] { GraphConstants.FilesReadWriteAppFolder };

        // Builds a FilesViewDisplayModel
        // folderId: ID of the folder to get files and folders from. If null, gets from the root
        // pageRequestUrl: Used for paging requests to get the next set of results
        public async Task<FilesViewDisplayModel> GetViewForFolder(string? folderId = null,
                                                           string? pageRequestUrl = null)
        {
            var model = new FilesViewDisplayModel();

            try
            {
                GraphServiceClient? graphClient = GetGraphClientForScopes(_filesScopes);

                // Get selected folder
                IDriveItemRequest folderRequest;
                if (string.IsNullOrEmpty(folderId))
                {
                    // Get the root
                    // GET /me/drive/root
                    folderRequest = graphClient.Me
                        .Drive
                        .Root
                        .Request();
                }
                else
                {
                    // GET /me/drive/items/folderId
                    folderRequest = graphClient.Me
                        .Drive
                        .Items[folderId]
                        .Request();
                }

                // Send the request
                model.SelectedFolder = await folderRequest
                    // Only select the fields used by the app
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.ParentReference
                    })
                    .GetAsync();

                // Get files and folders
                IDriveItemChildrenCollectionRequest itemRequest;

                // Is this a page request?
                if (!string.IsNullOrEmpty(pageRequestUrl))
                {
                    // Instead of using the request builders,
                    // initialize the request directly from the supplied
                    // URL
                    itemRequest = new DriveItemChildrenCollectionRequest(
                        pageRequestUrl, graphClient, null);
                }
                else if (string.IsNullOrEmpty(folderId))
                {
                    // No folder ID specified, so use /me/drive/root/children
                    // to get all items in the root of user's drive
                    // GET /me/drive/root/children
                    itemRequest = graphClient.Me
                        .Drive
                        .Special
                        .AppRoot
                        .Children
                        .Request();
                }
                else
                {
                    // Folder ID specified
                    // GET /me/drive/items/folderId/children
                    itemRequest = graphClient.Me
                        .Drive
                        .Items[folderId]
                        .Children
                        .Request();
                }

                if (string.IsNullOrEmpty(pageRequestUrl))
                {
                    itemRequest = itemRequest
                        .Top(GraphConstants.PageSize)
                        // Only get the fields used by the view
                        .Select(d => new
                        {
                            d.File,
                            d.FileSystemInfo,
                            d.Folder,
                            d.Id,
                            d.LastModifiedBy,
                            d.Name,
                            d.ParentReference
                        })
                        .Expand("thumbnails");
                }

                // Get max PageSize number of results
                IDriveItemChildrenCollectionPage? driveItemPage = await itemRequest
                    .GetAsync();

                model.Files = new List<DriveItem>();
                model.Folders = new List<DriveItem>();

                foreach (DriveItem? item in driveItemPage.CurrentPage)
                {
                    if (item.Folder != null)
                    {
                        model.Folders.Add(item);
                    }
                    else if (item.File != null)
                    {
                        model.Files.Add(item);
                    }
                }

                model.NextPageUrl = driveItemPage.NextPageRequest?.GetHttpRequestMessage()?.RequestUri?.ToString();

                return model;
            }
            catch (ServiceException ex)
            {
                InvokeAuthIfNeeded(ex);

                throw;
            }
        }
    }
}
