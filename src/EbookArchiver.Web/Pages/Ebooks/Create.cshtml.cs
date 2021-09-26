using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EbookArchiver.Web.Pages.Ebooks
{
    public class CreateModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public CreateModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context) => _context = context;

        public IActionResult OnGet()
        {
            AccountId = new SelectList(_context.Accounts, nameof(Account.AccountId), nameof(Account.DisplayName));
            BookId = new SelectList(_context.Books, nameof(Book.BookId), nameof(Book.Title));
            return Page();
        }

        [BindProperty]
        public Ebook Ebook { get; set; } = new();

        [BindProperty]
        [Display(Name = "DRM-Free File")]
        public IFormFile? DrmStrippedFile { get; set; }

        [BindProperty]
        [Display(Name = "Original File")]
        public IFormFile? OriginalFile { get; set; }

        public IEnumerable<SelectListItem> AccountId { get; set; } = Array.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> BookId { get; set; } = Array.Empty<SelectListItem>();

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyModel = new Ebook();

            if (ModelState.IsValid &&
                await TryUpdateModelAsync(
                    emptyModel,
                    nameof(Ebook),
                    b => b.BookId,
                    b => b.PublisherISBN13!,
                    b => b.PublisherVersion!,
                    b => b.VendorVersion!,
                    b => b.VendorBookIdentifier!,
                    b => b.AccountId!,
                    b => b.EbookSource,
                    b => b.EbookFormat
                )
            )
            {
                _context.Ebooks.Add(emptyModel);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
