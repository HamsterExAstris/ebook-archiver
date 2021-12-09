using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EbookArchiver.JNovelClub.Models;
using EbookArchiver.Models;
using EbookArchiver.OneDrive;
using Microsoft.Extensions.Configuration;

namespace EbookArchiver.JNovelClub
{
    public class SyncService
    {
        private readonly BookService _bookService;
        private readonly HttpClient _httpClient;
        private readonly ILibrary _library;

        public SyncService(BookService bookService, HttpClient httpClient, ILibrary library, IConfiguration configuration)
        {
            _bookService = bookService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["JNovelClub:ApiBaseAddress"]);

            // Set up the access token.
            string? tokenSetting = configuration["JNovelClub:AccessToken"];
            string? decodedToken = configuration.GetValue("JNovelClub:AccessTokenUrlDecode", true)
                ? WebUtility.UrlDecode(tokenSetting)
                : tokenSetting;
            // https://discord.com/channels/320950114355118090/320950114355118090/838822424253825044
            // decodeURIComponent(document.cookie.split("; ").find(row => row.startsWith('access_token=')).split('=')[1]).substring(2).split(".")[0]
            string? token = configuration.GetValue("JNovelClub:AccessTokenSplit", true)
                ? decodedToken.Substring(2).Split('.')[0] // Based on code in discor
                : decodedToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _library = library;
        }

        public async Task RunSync()
        {
            LibraryResponse? response = await _httpClient.GetFromJsonAsync<LibraryResponse>("me/library?format=json&include=serie");

            if (!(response?.Pagination?.LastPage ?? false))
            {
                throw new NotImplementedException("Multiple page support has not yet been implemented.");
            }

            IDictionary<string, string>? jncItems = await _bookService.GetJNovelClubEbooksAsync();

            foreach (JNovelClubPurchase jncPurchase in response.Books)
            {
                Series? series = null;
                if (jncPurchase.Series != null)
                {
                    series = _library.Series.FirstOrDefault(s => s.PublisherId == jncPurchase.Series.LegacyId);
                    if (series == null)
                    {
                        series = new Series
                        {
                            PublisherId = jncPurchase.Series.LegacyId
                        };
                        _library.AddNewSeries(series);
                    }
                    if (series.DisplayName != jncPurchase.Series.Title)
                    {
                        // Sync the title if it changed (or set it in the first place for a new record).
                        series.DisplayName = jncPurchase.Series.Title;
                    }
                }

                JNovelClubCreator? jncAuthor = jncPurchase.Volume.Creators.Where(c => c.Role == "AUTHOR")
                    .OrderBy(c => c.Name)
                    .FirstOrDefault();
                if (jncAuthor == null)
                {
                    // Can't find an author and we don't know how to deal with it, so just skip to next.
                    continue;
                }

                Author? author = _library.Authors.FirstOrDefault(a => a.DisplayName == jncAuthor.Name);
                if (author == null)
                {
                    author = new Author
                    {
                        DisplayName = jncAuthor.Name
                    };
                    _library.AddNewAuthor(author);
                }

                Book? book = _library.Books.FirstOrDefault(b => b.PublisherId == jncPurchase.Volume.LegacyId);
                if (book == null)
                {
                    book = new Book
                    {
                        PublisherId = jncPurchase.Volume.LegacyId,
                        // Do not overwrite title after creation so we can fix it if we want to.
                        Title = jncPurchase.Volume.Title,
                        // This is true for all of the default titles. We can uncheck as we update.
                        TitleDuplicatesSeriesData = true
                    };
                    _library.AddNewBook(book);
                }
                book.Author = author;
                book.Series = series;
                // Allow the Series Index to be fixed where we disagree with it.
                if (book.SeriesIndex == null)
                {
                    book.SeriesIndex = jncPurchase.Volume.Number.HasValue
                        ? jncPurchase.Volume.Number.ToString()
                        : null;
                }
                await _bookService.UpdateBookPathAsync(book);

                Ebook? ebook = null;
                if (book.BookId > 0)
                {
                    ebook = _library.Ebooks.FirstOrDefault(e => e.BookId == book.BookId);
                }
                if (ebook == null)
                {
                    ebook = new Ebook
                    {
                        EbookFormat = EbookFormat.Epub,
                        // Use the slug rather than the per-person unique ID.
                        VendorBookIdentifier = jncPurchase.Volume.Slug
                    };
                }
                if (book.Ebooks == null)
                {
                    book.Ebooks = new List<Ebook>();
                }
                book.Ebooks.Add(ebook);
                string? fileName = jncPurchase.Volume.Slug + ".epub";
                if (jncItems.TryGetValue(fileName, out string? oneDriveId))
                {
                    await _bookService.LinkEbookAsync(ebook, oneDriveId, null);
                }
            }

            await _library.SaveAsync();
        }
    }
}
