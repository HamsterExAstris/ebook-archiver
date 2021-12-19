using Ionic.Zip;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EbookArchiver.NookFixer
{
    internal class NookFixerService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _services;

        public NookFixerService(IConfiguration configuration, IServiceProvider services)
        {
            _configuration = configuration;
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string? targetPath = _configuration["outputPath"];
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                throw new InvalidOperationException("outputPath must be specified.");
            }

            // Instantiate the scope and the services from the scope. Dispoing the scope disposes of anything created from it, so we do not
            // need to dispose DbContext ourselves.
            using IServiceScope? scope = _services.CreateScope();
            DownloadDbContext? downloadDbContext = scope.ServiceProvider.GetRequiredService<DownloadDbContext>();

            await foreach (DownloadDocument? book in downloadDbContext.DownloadDocuments.Where(d => d.SavedFileName != string.Empty)
                .AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                Console.Write(Path.GetFileName(book.SavedFileName));
                if (File.Exists(book.SavedFileName))
                {
                    string? targetFile = Path.Combine(targetPath, Path.GetFileName(book.SavedFileName));
                    if (File.Exists(targetFile))
                    {
                        Console.WriteLine(" - already exists");
                    }
                    else
                    {
                        File.Copy(book.SavedFileName, targetFile);
                        if (!Path.GetExtension(targetFile).Equals(".epub", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine(" - not an EPUB");
                        }
                        else if (book.License != string.Empty)
                        {
                            RestoreLicense(targetFile, book.License);
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine(" - missing");
                }
            }

            LibraryDbContext? libraryDbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            using (var outfile = new StreamWriter(Path.Combine(targetPath, "library.csv")))
            {
                await outfile.WriteLineAsync("EAN,ISBN,Title,Series,SeriesIndex,Publisher");

                // Since duplicate titles exist, we need to handle the table separately rather than trying to set it up as a navigation property.
                Dictionary<string, LibraryProductV2Title>? titles = await libraryDbContext.Titles.Distinct().ToDictionaryAsync(k => k.Ean, v => v, cancellationToken);

                await foreach (LibraryProductV2? product in libraryDbContext.Products.Where(p => !p.Item.IsSampleEan)
                    .AsAsyncEnumerable().WithCancellation(cancellationToken))
                {
                    LibraryProductV2Title? title = titles.GetValueOrDefault(product.Ean);

                    string? seriesNumberText = string.IsNullOrWhiteSpace(product.SeriesNumber) || product.SeriesNumber == "0"
                        ? null
                        : product.SeriesNumber;

                    await outfile.WriteLineAsync($"\"\t{product.Ean}\",\"\t{product.Isbn}\",\"{title?.Title}\",\"{product.SeriesTitle}\",{seriesNumberText},\"{product.Publisher}\"");
                }
            }
        }

        private static void RestoreLicense(string epubPath, string licenseText)
        {
            using (var epub = new ZipFile(epubPath))
            {
                ZipEntry? rights = epub["META-INF\\rights.xml"];
                if (rights == null)
                {
                    epub.AddEntry("META-INF\\rights.xml", licenseText);
                    epub.Save();
                    Console.WriteLine(" - license restored");
                }
                else
                {
                    Console.WriteLine(" - license already exists");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
