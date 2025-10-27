using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportCentre.Data;
using SportCentre.Models;
using SportCentre.Models.ViewModels;

namespace SportCentre.Pages.SportCentres
{
    public class EditSportCentreModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;

        public EditSportCentreModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SportCentreEditViewModel viewModel { get; set; } = new SportCentreEditViewModel();


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centre =  await _context.SportCentres
                .Include(sp => sp.sportCentreAttivita)
                .ThenInclude(sca => sca.Attivita).FirstOrDefaultAsync(sp => sp.id == id);

            if (centre == null)
            {
                return NotFound();
            }

            viewModel.id = centre.id;
            viewModel.Name = centre.Name;
            viewModel.Address = centre.Address;
            viewModel.Location = centre.Location;
            viewModel.AvailableAttivita = await _context.attivita.ToListAsync();
            viewModel.existingAttivitaIds = centre.sportCentreAttivita!.Select(sca => sca.AttivitaId).ToList();

            //ViewData["attivitaId"] = new SelectList(_context.attivita, "Id", "Name");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            viewModel.AvailableAttivita = await _context.attivita.ToListAsync();
            var centre = await _context.SportCentres
                            .Include(sp => sp.sportCentreAttivita)
                            .ThenInclude(sca => sca.Attivita).FirstOrDefaultAsync(sp => sp.id == viewModel.id);

            if (centre == null)
            {
                return NotFound();
            }

            centre.Name = viewModel.Name;
            centre.Address = viewModel.Address;
            centre.Location = viewModel.Location;
            centre.sportCentreAttivita = new List<SportCentreAttivita>();
            foreach (var attivitaId in viewModel.existingAttivitaIds)
            {
                centre.sportCentreAttivita.Add(new SportCentreAttivita
                {
                    SportCentreId = centre.id,
                    AttivitaId = attivitaId,
                    CreatedAt = DateTime.UtcNow
                });
            }
            _context.Attach(centre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CentreExists(centre.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Page();
            //return RedirectToPage("./SPortCentresIndex");
        }

        private bool CentreExists(int id)
        {
            return _context.SportCentres.Any(e => e.id == id);
        }
    }
}
