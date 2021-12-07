using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbookArchiver.Web.Pages.Ebooks
{
    public class LinkModel : PageModel
    {
        public IActionResult OnGetAsync() => Page();
    }
}
