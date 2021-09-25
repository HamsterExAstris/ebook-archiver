using System;
using System.Collections.Generic;
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

        public DbSet<Ebook> Ebooks => Set<Ebook>();

        public DbSet<Series> Series => Set<Series>();

        IEnumerable<Account> ILibrary.Accounts => Accounts;

        IEnumerable<Author> ILibrary.Authors => Authors;

        IEnumerable<Book> ILibrary.Books => Books;

        IEnumerable<Ebook> ILibrary.Ebooks => Ebooks;

        IEnumerable<Series> ILibrary.Series => Series;

        public EbookArchiverDbContext(DbContextOptions options) : base(options)
        {
        }

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
                throw new InvalidOperationException(string.Format(ErrorReferenceDoesNotExist, "Ebook", "Book"));
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
        public Task Save() => SaveChangesAsync();
    }
}
