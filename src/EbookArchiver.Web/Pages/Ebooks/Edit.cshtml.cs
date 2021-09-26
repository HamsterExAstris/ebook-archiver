using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Ebooks
{
    public class EditModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public EditModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context) => _context = context;

        [BindProperty]
        public Ebook? Ebook { get; set; }

        public IEnumerable<SelectListItem> AccountId { get; set; } = Array.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> BookId { get; set; } = Array.Empty<SelectListItem>();

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
            AccountId = new SelectList(_context.Accounts, nameof(Account.AccountId), nameof(Account.DisplayName));
            BookId = new SelectList(_context.Books, nameof(Book.BookId), nameof(Book.Title));
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Ebook == null)
            {
                throw new InvalidOperationException(nameof(Ebook) + " must be populated before POST.");
            }

            _context.Attach(Ebook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EbookExists(Ebook.EbookId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EbookExists(int id) => _context.Ebooks.Any(e => e.EbookId == id);
    }
}
