using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.Web.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly EbookArchiver.Data.MySql.EbookArchiverDbContext _context;

        public IndexModel(EbookArchiver.Data.MySql.EbookArchiverDbContext context)
        {
            _context = context;
        }

        public IList<Account> Account { get; set; } = Array.Empty<Account>();

        public async Task OnGetAsync()
        {
            Account = await _context.Accounts.ToListAsync();
        }
    }
}
