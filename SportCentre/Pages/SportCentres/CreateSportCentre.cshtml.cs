using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportCentre.Data;
using SportCentre.Models;
using SportCentre.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SportCentre.Pages.SportCentres
{
    public class CreateSportCentreModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;

        [BindProperty]
        public SportCentreCreateViewModel Input { get; set; } = default!;

        public CreateSportCentreModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
            Input = new SportCentreCreateViewModel();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Input.AvailableAttivita = await _context.attivita.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var centro = new SportCentre.Models.SportCentre
            {
                Name = Input.Name,
                Address = Input.Address,
                Location = Input.Location
            };

            _context.SportCentres.Add(centro);
            await _context.SaveChangesAsync();


            // Handle many-to-many relationship
            foreach (var attivitaId in Input.SelectedAttivitaIds)
            {
                _context.SportCentreAttivita.Add(new SportCentreAttivita
                {
                    SportCentreId = centro.id,
                    AttivitaId = attivitaId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("SportCentresIndex");
        }
    }
}
