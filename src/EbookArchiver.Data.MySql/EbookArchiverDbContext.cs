using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Data.MySql
{
    public class EbookArchiverDbContext : DbContext
    {
        public DbSet<Account>? Accounts { get; set; }

        public EbookArchiverDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
