using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using EbookArchiver.OneDrive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

namespace EbookArchiver.Web.Pages.Books
{
    [AuthorizeForScopes(Scopes = new[] { GraphConstants.FilesReadWriteAppFolder })]
    public class EditModel : PageModel
    {
        private readonly BookService _bookService;
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public EditModel(BookService bookService,
            EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

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

            // Initialize the connection to OneDrive so a save doesn't fail.
            await _bookService.InitiializeAccessAsync();

            AuthorId = new SelectList(_context.Authors.OrderBy(a => a.DisplayName), nameof(Author.AuthorId), nameof(Author.DisplayName));
            SeriesId = new SelectList(_context.Series.OrderBy(a => a.DisplayName),
                nameof(EbookArchiver.Models.Series.SeriesId),
                nameof(EbookArchiver.Models.Series.DisplayName));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Read in the book. The author may have changed so we'll read the data for OneDrive separately.
            Book? modelToUpdate = await _context.Books.FindAsync(id);

            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid
                && await TryUpdateModelAsync(
                    modelToUpdate,
                    nameof(Book),
                    b => b.Title!,
                    b => b.AuthorId,
                    b => b.IsNotOwned,
                    b => b.SeriesId!,
                    b => b.SeriesIndex!))
            {
                // Get the information on the author that OneDrive will need.
                Author? author = await _context.Authors.FindAsync(modelToUpdate.AuthorId);
                if (author == null)
                {
                    throw new InvalidOperationException("AuthorId " + modelToUpdate.AuthorId + " not found.");
                }
                modelToUpdate.Author = author;

                // Move the folder in OneDrive if applicable.
                await _bookService.UpdateBookPathAsync(modelToUpdate);

                // Update the database.
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
