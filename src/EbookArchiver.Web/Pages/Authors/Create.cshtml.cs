using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbookArchiver.Web.Pages.Authors
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
        public Author Author { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyModel = new Author();

            if (ModelState.IsValid &&
                await TryUpdateModelAsync<Author>(
                emptyModel,
                nameof(Author),
                s => s.DisplayName))
            {
                _context.Authors.Add(emptyModel);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
