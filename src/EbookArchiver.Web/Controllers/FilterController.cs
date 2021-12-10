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
                books = books.Include(b => b.Series)
                    .Where(b => (b.Series != null && b.Series.DisplayName.Contains(text)) || b.Title.Contains(text));
            }

            return Json(books.AsEnumerable().OrderBy(b => b.DisplayName).Select(b => new { b.BookId, b.DisplayName }));
        }
    }
}
