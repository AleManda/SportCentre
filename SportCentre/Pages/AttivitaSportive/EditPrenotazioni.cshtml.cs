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

namespace SportCentre.Pages.AttivitaSportive
{
    public class EditPrenotazioniModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;

        public EditPrenotazioniModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Prenotazione Prenotazione { get; set; } = default!;

        [BindProperty]
        public PrenotazioneViewModel PrenotazioneVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prenotazione = await _context.prenotazioni
                .Include(p => p.Attivita)
                .Include(p => p.User).FirstOrDefaultAsync(m => m.Id == id);

            if (prenotazione == null)
            {
                return NotFound();
            }

            PrenotazioneVM = new PrenotazioneViewModel
            {
                Id = prenotazione.Id,
                userId = prenotazione.userId,
                attivitaId = prenotazione.attivitaId,
                Data = prenotazione.Data
            };


            ViewData["attivitaId"] = new SelectList(_context.attivita, "Id", "Name");
            ViewData["userId"] = new SelectList(_context.Users, "Id", "UserName");

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

            var prenotazione = await _context.prenotazioni
                .FirstOrDefaultAsync(m => m.Id == PrenotazioneVM.Id);

            if (prenotazione == null)
            {
                return NotFound();
            }

            CheckAvailability(PrenotazioneVM.attivitaId, PrenotazioneVM.userId, PrenotazioneVM.Data);


            prenotazione.userId = PrenotazioneVM.userId;
            prenotazione.attivitaId = PrenotazioneVM.attivitaId;
            prenotazione.Data = PrenotazioneVM.Data;
            prenotazione.User = await _context.Users.FindAsync(PrenotazioneVM.userId);
            prenotazione.Attivita = await _context.attivita.FindAsync(PrenotazioneVM.attivitaId);
            Prenotazione = prenotazione;


            _context.Attach(Prenotazione).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrenotazioneExists(Prenotazione.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./PrenotazioniIndex");
        }

        private bool PrenotazioneExists(int id)
        {
            return _context.prenotazioni.Any(e => e.Id == id); 
        }

        private void CheckAvailability(int attivitaid, string userid,DateOnly data)
        {
            var prenotazioniCount = _context.prenotazioni
                .Count(p => p.attivitaId == attivitaid && p.Data == data);

            var attivita = _context.attivita.Find(attivitaid);
            if (attivita == null)
            {
                ModelState.AddModelError(string.Empty, "Attività non trovata.");
                return;
            }

            if (prenotazioniCount >= attivita.Posti)
            {
                ModelState.AddModelError(string.Empty, "Non ci sono posti disponibili per questa attività in questa data.");
            }


            bool alreadyBooked = _context.prenotazioni.Any(p => p.Data == data
                                        && p.attivitaId == attivita.Id
                                        && p.userId == userid);

            if (alreadyBooked)
            {
                ModelState.AddModelError(string.Empty, "Hai già una prenotazione per questa data.");
            }
        }
    }
}
