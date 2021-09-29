using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using EbookArchiver.Models;
using EbookArchiver.OneDrive;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Web;

namespace EbookArchiver.Web.Pages.Ebooks
{
    [AuthorizeForScopes(Scopes = new[] { GraphConstants.FilesReadWriteAppFolder })]
    public class DownloadModel : PageModel
    {
        private readonly BookService _bookService;
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public DownloadModel(BookService bookService,
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

        public async Task<IActionResult> OnGetAsync(int? id, bool drmFree = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ebook = await _context.Ebooks.FindAsync(id);

            if (Ebook == null)
            {
                return NotFound();
            }

            string? itemId = drmFree ? Ebook.DrmStrippedFileId : Ebook.EbookFileId;
            if (itemId == null)
            {
                return NotFound();
            }

            string? fileName = drmFree ? Ebook.DrmStrippedFileName : Ebook.FileName;
            if (fileName == null)
            {
                throw new InvalidOperationException("File name not specified even though file ID was.");
            }

            Stream fileContents = await _bookService.DownloadEbookAsync(itemId);

            string mimeType;
            switch (Ebook.EbookFormat)
            {
                case EbookFormat.Mobipocket when Path.GetExtension(fileName).Equals(".prc", StringComparison.OrdinalIgnoreCase):
                case EbookFormat.Palm:
                    mimeType = "application/vnd.palm";
                    break;
                case EbookFormat.Mobipocket:
                    mimeType = "application/vnd.amazon.ebook";
                    break;
                case EbookFormat.KindleFormat7And8:
                case EbookFormat.KindleFormat8:
                    mimeType = "application/vnd.amazon.mobi8-ebook";
                    break;
                case EbookFormat.Epub:
                    mimeType = "application/epub+zip";
                    break;
                case EbookFormat.PortableDocumentFormat:
                    mimeType = "application/pdf";
                    break;
                default:
                    mimeType = "application/octet-stream";
                    break;
            }

            return new FileStreamResult(fileContents, mimeType)
            {
                FileDownloadName = fileName
            };
        }
    }
}
