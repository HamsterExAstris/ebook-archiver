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
    public class DetailsModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public DetailsModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        public Ebook? Ebook { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ebook = await _context.Ebooks
                .Include(e => e.Account)
                .Include(e => e.Book).FirstOrDefaultAsync(m => m.EbookId == id);

            if (Ebook == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
