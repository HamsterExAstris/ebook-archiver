using System.Linq;
using EbookArchiver.Data.MySql;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Controllers
{
    public class FilterController : Controller
    {
        private readonly EbookArchiverDbContext _library;

        public FilterController(EbookArchiverDbContext library) => _library = library;

        [HttpGet]
        public IActionResult Books(string text)
        {
            IQueryable<Book>? books = _library.Books;

            if (!string.IsNullOrEmpty(text))
            {
                books = books
                    .Include(b => b.Author)
                    .Include(b => b.Series)
                    .Where(b => b.Title.Contains(text)
                        || (b.Author != null && b.Author.DisplayName.Contains(text))
                        || ("#" + b.SeriesIndex).Contains(text)
                        || (b.Series != null && b.Series.DisplayName.Contains(text)));
            }

            // Need to cast to enumerable so we only try to access DisplayName locally.
            return Json(books.AsAsyncEnumerable().OrderBy(b => b.DisplayName).Select(b => new { b.BookId, b.DisplayName }));
        }
    }
}
