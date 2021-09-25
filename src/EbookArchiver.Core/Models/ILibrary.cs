using System.Collections.Generic;
using System.Threading.Tasks;

namespace EbookArchiver.Models
{
    public interface ILibrary
    {
        IEnumerable<Account> Accounts { get; }
        IEnumerable<Author> Authors { get; }
        IEnumerable<Book> Books { get; }
        IEnumerable<Ebook> Ebooks { get; }
        IEnumerable<Series> Series { get; }

        void AddNewAccount(Account newAccount);
        void AddNewAuthor(Author newAuthor);
        void AddNewBook(Book newBook);
        void AddNewEbook(Ebook newEbook);
        void AddNewSeries(Series newSeries);
        void DeleteBook(Book toDelete);
        void DeleteEbook(Ebook toDelete);
        Task Save();
    }
}