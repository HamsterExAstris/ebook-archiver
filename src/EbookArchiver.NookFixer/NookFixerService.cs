using Ionic.Zip;
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

        public Task StartAsync(CancellationToken cancellationToken)
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

            foreach (DownloadDocument? book in downloadDbContext.DownloadDocuments.Where(d => d.SavedFileName != string.Empty))
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
                        if (book.License != string.Empty)
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

            return Task.CompletedTask;
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
