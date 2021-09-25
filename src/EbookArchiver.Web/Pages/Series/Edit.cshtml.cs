using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbookArchiver.Web.Pages.Series
{
    public class EditModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public EditModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EbookArchiver.Models.Series? Series { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Series = await _context.Series.FindAsync(id);

            if (Series == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var modelToUpdate = await _context.Series.FindAsync(id);

            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid
                && await TryUpdateModelAsync<EbookArchiver.Models.Series>(
                    modelToUpdate,
                    nameof(Series),
                    m => m.DisplayName))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
