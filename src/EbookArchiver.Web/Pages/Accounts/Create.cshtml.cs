﻿using System.Threading.Tasks;
using EbookArchiver.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbookArchiver.Web.Pages.Accounts
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
        public Account Account { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyModel = new Account();

            if (ModelState.IsValid &&
                await TryUpdateModelAsync<Account>(
                emptyModel,
                nameof(Account),
                s => s.DisplayName))
            {
                _context.Accounts.Add(emptyModel);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
