using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EbookArchiver.Data.MySql;
using EbookArchiver.Models;

namespace EbookArchiver.Web.Pages.Ebooks
{
    public class IndexModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public IndexModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        public IList<Ebook> Ebook { get; set; } = Array.Empty<Ebook>();

        public async Task OnGetAsync()
        {
            Ebook = await _context.Ebooks
                .Include(e => e.Account)
                .Include(e => e.Book).ToListAsync();
        }
    }
}
