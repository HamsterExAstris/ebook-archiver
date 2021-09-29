using System;
using System.Collections.Generic;
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

        public async Task OnGetAsync() => Book = await _context.SortedBooks
            .Include(b => b.Author)
            .Include(b => b.Series)
            .ToListAsync();
    }
}
