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


        //
        //___________________________________________________________________________________________
        public async Task<IActionResult> OnGetAsync()
        {
            Input.AvailableAttivita = await _context.attivita.ToListAsync();
            return Page();
        }


        //
        //___________________________________________________________________________________________
        public async Task<IActionResult> OnPostAsync()
        {
            // unicità nome
            if (await _context.SportCentres.AnyAsync(sc => sc.Name == Input.Name))
            {
                ModelState.AddModelError("Input.Name", "Esiste già un centro con questo nome.");
            }

            // selected ids validi
            var dbIds = await _context.attivita.Select(a => a.Id).ToListAsync();
            var invalid = (Input.SelectedAttivitaIds ?? new List<int>()).Except(dbIds).ToList();
            if (invalid.Any())
            {
                ModelState.AddModelError("Input.SelectedAttivitaIds", "Sono state selezionate attività non valide.");
            }




            // If model validation fails (including IValidatableObject.Validate),
            // repopulate AvailableAttivita before returning the page so checkboxes are shown.
            if (!ModelState.IsValid)
            {
                Input.AvailableAttivita = await _context.attivita.ToListAsync();
                return Page();
            }


            try
            {
                //nuovo centro sportivo
                var centro = new SportCentre.Models.SportCentre
                {
                    Name = Input.Name,
                    Address = Input.Address,
                    Location = Input.Location
                };

                _context.SportCentres.Add(centro);
                //save perchè mi serve l'id per la many to many
                await _context.SaveChangesAsync();


                // Handle many-to-many relationship
                if (Input.SelectedAttivitaIds != null)
                {
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
                }
            }
            catch (DbUpdateException)
            {
                // errore globale (model-level)
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante il salvataggio. Riprova più tardi.");
                Input.AvailableAttivita = await _context.attivita.ToListAsync();
                return Page();
            }

            return RedirectToPage("SportCentresIndex");
        }
    }
}
