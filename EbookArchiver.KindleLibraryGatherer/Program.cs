using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EbookArchiver.KindleLibraryGatherer;

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
};

var items = new List<LibraryItem>();
if (File.Exists("output.json"))
{
    using var stream = new FileStream("output.json", FileMode.Open);
    LibraryResult? result = await JsonSerializer.DeserializeAsync<LibraryResult>(stream, options);
    if (result != null)
    {
        items.AddRange(result.ItemsList);
    }
}
else
{
    var handler = new HttpClientHandler()
    {
        UseCookies = true,
        CookieContainer = new CookieContainer()
    };

    using var client = new HttpClient(handler, true);

    // Cookies and other private headers go here. Currently they are just saved in secrets.json; still need to JSON-ify them and read them.

    client.DefaultRequestHeaders.UserAgent.ParseAdd("");

    // It's possible this list can be filtered down some.

    handler.CookieContainer.Add(new Cookie("session-id", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("i18n-prefs", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("ubid-main", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("lc-main", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("sid", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("session-token", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("x-main", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("at-main", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("sess-at-main", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("session-id-time", "", "/", ".amazon.com"));
    handler.CookieContainer.Add(new Cookie("csm-hit", "", "/", "read.amazon.com"));

    client.DefaultRequestHeaders.Add("x-amzn-sessionid", "");

    client.DefaultRequestHeaders.Add("Referer", "https://read.amazon.com/kindle-library?tabView=all");
    client.DefaultRequestHeaders.Add("Accept", "*/*");

    LibraryResult? result = null;
    do
    {
        var uri = new Uri($"https://read.amazon.com/kindle-library/search?query=&libraryType=BOOKS&paginationToken={result?.PaginationToken ?? "0"}&sortType=recency&querySize=50");
        result = await client.GetFromJsonAsync<LibraryResult>(uri, options);
        if (result != null)
        {
            items.AddRange(result.ItemsList);
        }
    } while (result?.PaginationToken != null);

    using var stream = new FileStream("output.json", FileMode.Create);
    var fileResult = new LibraryResult
    {
        ItemsList = items.ToArray()
    };
    await JsonSerializer.SerializeAsync(stream, fileResult, options);
}


/*-Headers @{
    "Cache-Control" = "max-age=0"
  "rtt" = "100"
  "downlink" = "10"
  "ect" = "4g"
  "sec-ch-ua" = "`" Not A; Brand`";v=`"99`", `"Chromium`";v=`"96`", `"Microsoft Edge`";v=`"96`""
  "sec-ch-ua-mobile" = "?0"
  "sec-ch-ua-platform" = "`"Windows`""
  "DNT" = "1"
  "Upgrade-Insecure-Requests" = "1"*/
// "Accept" = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"
/* "Sec-Fetch-Site" = "none"
 "Sec-Fetch-Mode" = "navigate"
 "Sec-Fetch-User" = "?1"
 "Sec-Fetch-Dest" = "document"
 "Accept-Encoding" = "gzip, deflate, br"
 "Accept-Language" = "en-US,en;q=0.9"
}*/

using var writer = new StreamWriter("output.csv");
using var sqlWriter = new StreamWriter("output.sql");
await sqlWriter.WriteLineAsync("START TRANSACTION;");
foreach (LibraryItem? item in items)
{
    await OutputObject(item, writer);
    await OutputSql(item, sqlWriter);
}
await sqlWriter.WriteLineAsync("ROLLBACK;");
await sqlWriter.WriteLineAsync("--COMMIT;");

static async Task OutputObject(LibraryItem item, StreamWriter writer)
{
    await writer.WriteAsync('"');
    await writer.WriteAsync(item.Title);
    await writer.WriteAsync("\",\"");
    await writer.WriteAsync(string.Join(';', item.Authors));
    await writer.WriteAsync("\",\"");
    await writer.WriteAsync(item.ASIN);
    await writer.WriteAsync("\",\"");
    await writer.WriteAsync(item.MangaAsin.ToString());
    await writer.WriteAsync("\",\"");
    await writer.WriteAsync(item.ResourceType);
    await writer.WriteAsync("\",\"");
    await writer.WriteAsync(item.PercentageRead.ToString(CultureInfo.InvariantCulture));
    await writer.WriteLineAsync('"');
}

static async Task OutputSql(LibraryItem item, StreamWriter writer)
{
    if (item.ResourceType != "EBOOK")
    {
        return;
    }

    string? authorName = item.Authors?.FirstOrDefault()?.Replace("'", "''");
    if (authorName != null && authorName.EndsWith(':'))
    {
        authorName = authorName.Substring(0, authorName.Length - 1);
    }
    await writer.WriteLineAsync($"SELECT @AuthorId := MIN(AuthorId) FROM `Authors` WHERE DisplayName = '{authorName}';");
    await writer.WriteLineAsync("SET @AuthorId := IF(@AuthorId, @AuthorId, (SELECT MIN(AuthorId) FROM `authors` WHERE DisplayName LIKE '_reuse%'));");
    await writer.WriteLineAsync($"UPDATE`Authors` SET DisplayName = '{authorName}' WHERE AuthorId = @AuthorId;");
    await writer.WriteLineAsync($"INSERT INTO `authors` (DisplayName) SELECT * FROM (SELECT '{authorName}') a WHERE @AuthorId IS NULL;");
    await writer.WriteLineAsync($"SELECT @AuthorId := MIN(AuthorId) FROM `Authors` WHERE DisplayName = '{authorName}';");
    await writer.WriteLineAsync($"INSERT INTO `Books` (AuthorId, Title, IsNotOwned) VALUES (@AuthorId, '{item.Title.Replace("'", "''")}', 0);");
}
