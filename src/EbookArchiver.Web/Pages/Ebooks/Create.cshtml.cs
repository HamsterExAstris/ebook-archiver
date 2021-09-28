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
    public class CreateModel : PageModel
    {
        private readonly BookService _bookService;
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public CreateModel(BookService bookService,
            EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        public IActionResult OnGet()
        {
            AccountId = new SelectList(_context.Accounts.OrderBy(a => a.DisplayName), nameof(Account.AccountId), nameof(Account.DisplayName));
            BookId = new SelectList(_context.Books.OrderBy(b => b.Title), nameof(Book.BookId), nameof(Book.Title));
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
                if (OriginalFile != null)
                {
                    emptyModel.FileName = Path.GetFileName(OriginalFile.FileName);
                }

                if (DrmStrippedFile != null)
                {
                    emptyModel.DrmStrippedFileName = Path.GetFileName(DrmStrippedFile.FileName);
                }

                // Get the information on the book that OneDrive will need.
                Book? book = await _context.Books
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.BookId == emptyModel.BookId);
                if (book == null)
                {
                    throw new InvalidOperationException("BookId " + emptyModel.BookId + " not found in database.");
                }
                emptyModel.Book = book;

                // Upload the files to OneDrive.
                if (OriginalFile != null)
                {
                    using Stream originalStream = OriginalFile.OpenReadStream();
                    if (DrmStrippedFile != null)
                    {
                        using Stream drmStrippedStream = DrmStrippedFile.OpenReadStream();
                        await _bookService.UploadEbookAsync(emptyModel, originalStream, drmStrippedStream);
                    }
                    else
                    {
                        await _bookService.UploadEbookAsync(emptyModel, originalStream, null);
                    }
                }
                else if (DrmStrippedFile != null)
                {
                    using Stream drmStrippedStream = DrmStrippedFile.OpenReadStream();
                    await _bookService.UploadEbookAsync(emptyModel, null, drmStrippedStream);
                }

                // Write the changes to the database.
                _context.Ebooks.Add(emptyModel);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
