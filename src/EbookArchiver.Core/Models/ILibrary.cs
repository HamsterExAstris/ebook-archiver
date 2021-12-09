using System.Linq;
using System.Threading.Tasks;

namespace EbookArchiver.Models
{
    public interface ILibrary
    {
        IQueryable<Account> Accounts { get; }
        IQueryable<Author> Authors { get; }
        IQueryable<Book> Books { get; }
        IQueryable<Ebook> Ebooks { get; }
        IQueryable<Series> Series { get; }

        void AddNewAccount(Account newAccount);
        void AddNewAuthor(Author newAuthor);
        void AddNewBook(Book newBook);
        void AddNewEbook(Ebook newEbook);
        void AddNewSeries(Series newSeries);
        void DeleteBook(Book toDelete);
        void DeleteEbook(Ebook toDelete);
        Task SaveAsync();
    }
}
