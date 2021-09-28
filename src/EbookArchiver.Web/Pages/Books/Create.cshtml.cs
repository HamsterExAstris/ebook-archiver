using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EbookArchiver.Web.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public CreateModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context) => _context = context;

        public IActionResult OnGet()
        {
            AuthorId = new SelectList(_context.Authors.OrderBy(a => a.DisplayName), nameof(Author.AuthorId), nameof(Author.DisplayName));
            SeriesId = new SelectList(_context.Series.OrderBy(a => a.DisplayName),
                nameof(EbookArchiver.Models.Series.SeriesId),
                nameof(EbookArchiver.Models.Series.DisplayName));
            return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = new();

        public IEnumerable<SelectListItem> AuthorId { get; set; } = Array.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> SeriesId { get; set; } = Array.Empty<SelectListItem>();

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyModel = new Book();

            if (ModelState.IsValid &&
                await TryUpdateModelAsync(
                    emptyModel,
                    nameof(Book),
                    b => b.Title,
                    b => b.AuthorId,
                    b => b.IsNotOwned,
                    b => b.SeriesId!,
                    b => b.SeriesIndex!
                )
            )
            {
                // Get the information on the author that OneDrive will need.
                Author? author = await _context.Authors.FindAsync(emptyModel.AuthorId);
                if (author == null)
                {
                    throw new InvalidOperationException("AuthorId " + emptyModel.AuthorId + " not found.");
                }
                emptyModel.Author = author;

                _context.Books.Add(emptyModel);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
