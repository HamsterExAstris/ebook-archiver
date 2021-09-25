using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbookArchiver.Web.Pages.Accounts
{
    public class EditModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public EditModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Account? Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Account = await _context.Accounts.FindAsync(id);

            if (Account == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var modelToUpdate = await _context.Accounts.FindAsync(id);

            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid
                && await TryUpdateModelAsync<Account>(
                    modelToUpdate,
                    nameof(Account),
                    m => m.DisplayName))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
