using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Series
{
    public class DeleteModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public DeleteModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EbookArchiver.Models.Series Series { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Series = await _context.Series.FirstOrDefaultAsync(m => m.SeriesId == id);

            if (Series == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Series = await _context.Series.FindAsync(id);

            if (Series != null)
            {
                _context.Series.Remove(Series);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
