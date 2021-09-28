using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public IndexModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context) => _context = context;

        public IList<Book> Book { get; set; } = Array.Empty<Book>();

        public async Task OnGetAsync() => Book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Series)
            .OrderBy(b => b.Author!.DisplayName)
            .ThenBy(b => b.Series != null ? b.Series.DisplayName : string.Empty)
            .ThenBy(b => b.SeriesIndex)
            .ThenBy(b => b.Title)
            .ToListAsync();
    }
}
