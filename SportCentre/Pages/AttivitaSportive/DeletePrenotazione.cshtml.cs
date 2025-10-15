using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SportCentre.Data;
using SportCentre.Models;

namespace SportCentre.Pages.AttivitaSportive
{
    public class DeletePrenotazioneModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        public string deletemsg = "";

        public DeletePrenotazioneModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Prenotazione Prenotazione { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prenotazione = await _context.prenotazioni.Include(p => p.User).Include(p => p.Attivita).FirstOrDefaultAsync(m => m.Id == id);

            if (prenotazione is not null)
            {
                Prenotazione = prenotazione;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prenotazione = await _context.prenotazioni.FindAsync(id);
            if (prenotazione != null)
            {
                Prenotazione = prenotazione;
                _context.prenotazioni.Remove(Prenotazione);
                await _context.SaveChangesAsync();
            }
            deletemsg = "Prenotazione eliminata con successo.";
            return Page();
            //return RedirectToPage("./PrenotazioniIndex");
        }
    }
}
