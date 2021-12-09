using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Data.MySql
{
    public class EbookArchiverDbContext : DbContext, ILibrary
    {
        private const string ErrorPrimaryKeyAlreadySet
            = "Cannot add an item that already has a primary key set - it must exist in the library!";

        private const string ErrorReferenceDoesNotExist
            = "Cannot add an {0} without its referenced {1} existing in the library!";

        public DbSet<Account> Accounts => Set<Account>();

        public DbSet<Author> Authors => Set<Author>();

        public DbSet<Book> Books => Set<Book>();

        /// <summary>
        /// Sort the books by <see cref="Book.DisplayName"/>.
        /// </summary>
        public IOrderedQueryable<Book> SortedBooks => Books
            .OrderBy(b => b.Author!.DisplayName)
            .ThenBy(b => b.Series != null ? b.Series.DisplayName : string.Empty)
            .ThenBy(b => b.SeriesIndex)
            .ThenBy(b => b.Title);

        public IQueryable<Book> SortedBooksAndChildren => SortedBooks
            .Include(b => b!.Series)
            .Include(b => b!.Author);

        /// <summary>
        /// Sort the books by <see cref="Book.DisplayName"/>.
        /// </summary>
        public IQueryable<Ebook> SortedEbooks => Books
            .OrderBy(b => b.Author!.DisplayName)
            .ThenBy(b => b.Series != null ? b.Series.DisplayName : string.Empty)
            .ThenBy(b => b.SeriesIndex)
            .ThenBy(b => b.Title)
            // EF will always populate this, but we can't allow it to be non-nullable because a default value breaks population.
            .SelectMany(b => b.Ebooks!.OrderBy(e => e.EbookId));

        public IQueryable<Ebook> SortedEbooksAndChildren => SortedEbooks
            .Include(e => e.Account)
            .Include(e => e.Book)
            .ThenInclude(b => b!.Series)
            .Include(e => e.Book)
            .ThenInclude(b => b!.Author);

        public DbSet<Ebook> Ebooks => Set<Ebook>();

        public DbSet<Series> Series => Set<Series>();

        IQueryable<Account> ILibrary.Accounts => Accounts;

        IQueryable<Author> ILibrary.Authors => Authors;

        IQueryable<Book> ILibrary.Books => Books;

        IQueryable<Ebook> ILibrary.Ebooks => Ebooks;

        IQueryable<Series> ILibrary.Series => Series;

        public EbookArchiverDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.LogTo(message => Debug.WriteLine(message));

        void ILibrary.AddNewAccount(Account newAccount)
        {
            if (newAccount.AccountId > 0)
            {
                throw new InvalidOperationException(ErrorPrimaryKeyAlreadySet);
            }

            Accounts?.Add(newAccount);
        }

        void ILibrary.AddNewAuthor(Author newAuthor)
        {
            if (newAuthor.AuthorId > 0)
            {
                throw new InvalidOperationException(ErrorPrimaryKeyAlreadySet);
            }

            Authors?.Add(newAuthor);
        }

        void ILibrary.AddNewBook(Book newBook)
        {
            if (newBook.BookId > 0)
            {
                throw new InvalidOperationException(ErrorPrimaryKeyAlreadySet);
            }

            Books.Add(newBook);
        }

        void ILibrary.AddNewEbook(Ebook newEbook)
        {
            if (!Books.Any(b => b.BookId == newEbook.BookId))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ErrorReferenceDoesNotExist, "Ebook", "Book"));
            }

            Ebooks.Add(newEbook);
        }

        void ILibrary.AddNewSeries(Series newSeries)
        {
            if (newSeries.SeriesId > 0)
            {
                throw new InvalidOperationException(ErrorPrimaryKeyAlreadySet);
            }

            Series.Add(newSeries);
        }

        /// <remarks>
        /// Assuming that when we delete this it will cascade, and that we don't
        /// need to do it manually.
        /// </remarks>
        public void DeleteBook(Book toDelete) => Books.Remove(toDelete);

        public void DeleteEbook(Ebook toDelete) => Ebooks.Remove(toDelete);
        public Task SaveAsync() => SaveChangesAsync();
    }
}
