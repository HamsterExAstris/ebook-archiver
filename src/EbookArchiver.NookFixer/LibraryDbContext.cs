using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.NookFixer
{
    internal class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<LibraryProductV2> Products { get; set; } = null!;

        public DbSet<LibraryProductV2Title> Titles { get; set; } = null!;
    }
}
