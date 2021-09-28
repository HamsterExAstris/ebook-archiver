using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Series
{
    public class IndexModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public IndexModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context) => _context = context;

        public IList<EbookArchiver.Models.Series> Series { get; set; } = Array.Empty<EbookArchiver.Models.Series>();

        public async Task OnGetAsync() => Series = await _context.Series.OrderBy(s => s.DisplayName).ToListAsync();
    }
}
