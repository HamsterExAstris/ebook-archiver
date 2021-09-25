using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbookArchiver.Web.Pages.Authors
{
    public class EditModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public EditModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Author Author { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Author = await _context.Authors.FindAsync(id);

            if (Author == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var modelToUpdate = await _context.Authors.FindAsync(id);

            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid
                && await TryUpdateModelAsync<Author>(
                    modelToUpdate,
                    nameof(Author),
                    m => m.DisplayName))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
