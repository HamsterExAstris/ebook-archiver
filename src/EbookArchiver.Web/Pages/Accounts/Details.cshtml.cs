using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Accounts
{
    public class DetailsModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public DetailsModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        public Account Account { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountId == id);

            if (Account == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
