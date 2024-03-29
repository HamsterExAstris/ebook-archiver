﻿using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.NookFixer
{
    internal class DownloadDbContext : DbContext
    {
        public DownloadDbContext(DbContextOptions<DownloadDbContext> options) : base(options)
        {
        }

        public DbSet<DownloadDocument> DownloadDocuments { get; set; } = null!;
    }
}
