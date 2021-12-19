using EbookArchiver.NookFixer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((c, s) =>
    {
        string? nookPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Packages",
                c.Configuration.GetValue("NookAppPackageName", "BarnesNoble.Nook_ahnzqzva31enc"),
                "LocalState"
            );

        s.AddDbContext<DownloadDbContext>(o =>
        {
            string? downloadsDb = Path.Combine(nookPath, "NookDownloads.db3");
            var connStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = downloadsDb
            };
            o.UseSqlite(connStringBuilder.ToString());
        });

        s.AddDbContext<LibraryDbContext>(o =>
        {
            string? libraryDb = Path.Combine(nookPath, "NookLibrary.db3");
            var connStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = libraryDb
            };
            o.UseSqlite(connStringBuilder.ToString());
        });

        s.AddHostedService<NookFixerService>();
    })
    .RunConsoleAsync();
