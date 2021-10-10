using System.Globalization;
using EbookArchiver.Data.MySql;
using EbookArchiver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

IConfigurationRoot? config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddUserSecrets<Program>()
    .Build();

string? filePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "Fallen Temple Ebook Archiver",
    "Datastores for Fallen Temple Ebook Archiver.csv"
    );

var optionsBuilder = new DbContextOptionsBuilder<EbookArchiverDbContext>();
string? connectionString = config.GetConnectionString("localdb");
optionsBuilder.UseMySql(connectionString, serverVersion: ServerVersion.AutoDetect(connectionString));

var accounts = new List<Account>();
var authors = new List<Author>();
var books = new List<Book>();
var ebooks = new List<Ebook>();
var seriesList = new List<Series>();

using (var parser = new TextFieldParser(filePath))
{
    parser.TextFieldType = FieldType.Delimited;
    parser.SetDelimiters(",");
    parser.HasFieldsEnclosedInQuotes = true;

    while (!parser.EndOfData)
    {
        string[]? currentRow = parser.ReadFields();
        if (currentRow == null
            || (currentRow.Length != 3 || currentRow[0] == "Table"))
        {
            // Header.
            continue;
        }

        switch (currentRow[0])
        {
            case "Account":
                Account? account = JsonConvert.DeserializeObject<Account>(currentRow[2]);
                if (account != null)
                {
                    account.AccountId = int.Parse(currentRow[1], CultureInfo.InvariantCulture);
                    accounts.Add(account);
                }
                break;
            case "Author":
                Author? author = JsonConvert.DeserializeObject<Author>(currentRow[2]);
                if (author != null)
                {
                    author.AuthorId = int.Parse(currentRow[1], CultureInfo.InvariantCulture);
                    authors.Add(author);
                }
                break;
            case "Book":
                Book? book = JsonConvert.DeserializeObject<Book>(currentRow[2]);
                if (book != null)
                {
                    book.BookId = int.Parse(currentRow[1], CultureInfo.InvariantCulture);
                    books.Add(book);
                }
                break;
            case "Ebook":
                Ebook? ebook = JsonConvert.DeserializeObject<Ebook>(currentRow[2]);
                if (ebook != null)
                {
                    ebook.EbookId = int.Parse(currentRow[1], CultureInfo.InvariantCulture);
                    ebooks.Add(ebook);
                }
                break;
            case "Series":
                Series? series = JsonConvert.DeserializeObject<Series>(currentRow[2]);
                if (series != null)
                {
                    series.SeriesId = int.Parse(currentRow[1], CultureInfo.InvariantCulture);
                    seriesList.Add(series);
                }
                break;
        }
    }
}

using (var context = new EbookArchiverDbContext(optionsBuilder.Options))
{
    context.Database.Migrate();

    context.Accounts.AddRange(accounts);
    context.Authors.AddRange(authors);
    context.Series.AddRange(seriesList);
    context.Books.AddRange(books);

    // Actually importing the ebooks fails with foreign key constraints for some reason, but I think I want to start over there anyways.

    await context.SaveChangesAsync();
}
