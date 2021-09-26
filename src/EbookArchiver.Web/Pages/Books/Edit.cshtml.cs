using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public EditModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context) => _context = context;

        [BindProperty]
        public Book? Book { get; set; }

        public IEnumerable<SelectListItem> AuthorId { get; set; } = Array.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> SeriesId { get; set; } = Array.Empty<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Series).FirstOrDefaultAsync(m => m.BookId == id);

            if (Book == null)
            {
                return NotFound();
            }
            AuthorId = new SelectList(_context.Authors, nameof(Author.AuthorId), nameof(Author.DisplayName));
            SeriesId = new SelectList(_context.Series,
                nameof(EbookArchiver.Models.Series.SeriesId),
                nameof(EbookArchiver.Models.Series.DisplayName));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Book? modelToUpdate = await _context.Books.FindAsync(id);

            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid
                && await TryUpdateModelAsync(
                    modelToUpdate,
                    nameof(Book),
                    b => b.Title,
                    b => b.AuthorId,
                    b => b.IsNotOwned,
                    b => b.SeriesId!,
                    b => b.SeriesIndex!))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
