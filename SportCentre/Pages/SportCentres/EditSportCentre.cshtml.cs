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
        public SportCentre.Models.SportCentre Centre { get; set; } = default!;

        [BindProperty]
        public Attivita Attivita { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centre =  await _context.SportCentres.FirstOrDefaultAsync(m => m.id == id);
            if (centre == null)
            {
                return NotFound();
            }
            Centre = centre;
            ViewData["attivitaId"] = new SelectList(_context.attivita, "Id", "Name");

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

            var attivita = await _context.attivita.FirstOrDefaultAsync(a => a.Id == Attivita.Id);   
            Centre.AttivitaSportive.Add(attivita!);

            _context.Attach(Centre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CentreExists(Centre.id))
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
