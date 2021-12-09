using System.Threading.Tasks;
using EbookArchiver.JNovelClub;
using EbookArchiver.OneDrive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace EbookArchiver.Web.Pages.Sync
{
    [AuthorizeForScopes(Scopes = new[] { GraphConstants.FilesReadWriteAppFolder })]
    public class JNovelClubModel : PageModel
    {
        private readonly BookService _bookService;
        private readonly SyncService _syncService;

        public JNovelClubModel(BookService bookService, SyncService syncService)
        {
            _bookService = bookService;
            _syncService = syncService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Initialize the connection to OneDrive so a save doesn't fail.
            await _bookService.InitiializeAccessAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _syncService.RunSync();
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
