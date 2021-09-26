using System.Threading.Tasks;
using EbookArchiver.OneDrive;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace EbookArchiver.Web.Pages
{
    [AuthorizeForScopes(Scopes = new[] { GraphConstants.FilesReadWriteAppFolder })]
    public class OneDriveTestModel : PageModel
    {
        private readonly OneDriveService _oneDriveService;

        public OneDriveTestModel(OneDriveService oneDriveService) => _oneDriveService = oneDriveService;

        public FilesViewDisplayModel Files { get; set; } = new();

        public async Task OnGet() => Files = await _oneDriveService.GetViewForFolder();
    }
}
