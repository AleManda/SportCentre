using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportCentre.Data;
using SportCentre.Models;

namespace SportCentre.Pages.SportCentres
{
    public class CreateSportCentreModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;

        public CreateSportCentreModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Centre scentre { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SportCentres.Add(scentre);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
