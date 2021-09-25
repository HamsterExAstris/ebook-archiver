using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Authors
{
    public class DeleteModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public DeleteModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
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

            Author = await _context.Authors.FirstOrDefaultAsync(m => m.AuthorId == id);

            if (Author == null)
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

            Author = await _context.Authors.FindAsync(id);

            if (Author != null)
            {
                _context.Authors.Remove(Author);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
