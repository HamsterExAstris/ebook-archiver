using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbookArchiver.Web.Pages.Series
{
    public class CreateModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public CreateModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public EbookArchiver.Models.Series Series { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyModel = new EbookArchiver.Models.Series();

            if (ModelState.IsValid &&
                await TryUpdateModelAsync<EbookArchiver.Models.Series>(
                emptyModel,
                nameof(Series),
                s => s.DisplayName))
            {
                _context.Series.Add(emptyModel);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
