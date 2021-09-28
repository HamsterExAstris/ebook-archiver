using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Authors
{
    public class IndexModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public IndexModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context) => _context = context;

        public IList<Author> Author { get; set; } = Array.Empty<Author>();

        public async Task OnGetAsync() => Author = await _context.Authors.OrderBy(a => a.DisplayName).ToListAsync();
    }
}
