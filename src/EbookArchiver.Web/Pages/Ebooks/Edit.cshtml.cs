using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using EbookArchiver.OneDrive;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

namespace EbookArchiver.Web.Pages.Ebooks
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
        public Ebook? Ebook { get; set; }

        [BindProperty]
        [Display(Name = "DRM-Free File")]
        public IFormFile? DrmStrippedFile { get; set; }

        [BindProperty]
        [Display(Name = "Original File")]
        public IFormFile? OriginalFile { get; set; }

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
            AccountId = new SelectList(_context.Accounts.OrderBy(a => a.DisplayName), nameof(Account.AccountId), nameof(Account.DisplayName));
            BookId = new SelectList(_context.Books.OrderBy(a => a.Title), nameof(Book.BookId), nameof(Book.Title));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // Read in the book. BookId can change so we'll populate data later.
            Ebook? modelToUpdate = await _context.Ebooks.FindAsync(id);

            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid
                && await TryUpdateModelAsync(
                    modelToUpdate,
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
                if (OriginalFile != null)
                {
                    modelToUpdate.FileName = Path.GetFileName(OriginalFile.FileName);
                }

                if (DrmStrippedFile != null)
                {
                    modelToUpdate.DrmStrippedFileName = Path.GetFileName(DrmStrippedFile.FileName);
                }

                // Get the information on the book that OneDrive will need.
                Book? book = await _context.Books
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.BookId == modelToUpdate.BookId);
                if (book == null)
                {
                    throw new InvalidOperationException("BookId " + modelToUpdate.BookId + " not found in database.");
                }
                modelToUpdate.Book = book;

                // Upload the files to OneDrive.
                if (OriginalFile != null)
                {
                    using Stream originalStream = OriginalFile.OpenReadStream();
                    if (DrmStrippedFile != null)
                    {
                        using Stream drmStrippedStream = DrmStrippedFile.OpenReadStream();
                        await _bookService.UploadEbookAsync(modelToUpdate, originalStream, drmStrippedStream);
                    }
                    else
                    {
                        await _bookService.UploadEbookAsync(modelToUpdate, originalStream, null);
                    }
                }
                else if (DrmStrippedFile != null)
                {
                    using Stream drmStrippedStream = DrmStrippedFile.OpenReadStream();
                    await _bookService.UploadEbookAsync(modelToUpdate, null, drmStrippedStream);
                }

                // Update the database.
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
