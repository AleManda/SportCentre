using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportCentre.Data;
using SportCentre.Models;
using SportCentre.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportCentre.Pages.AttivitaSportive
{
    [Authorize(Roles = "Admin")]
    public class EditPrenotazioniModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public PrenotazioneViewModel PrenotazioneVM { get; set; } = new PrenotazioneViewModel();
        public string? MessaggioConferma { get; set; }


        public EditPrenotazioniModel(SportCentre.Data.ApplicationDbContext context,UserManager<IdentityUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        //
        //___________________________________________________________________________________________
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prenotazione = await _context.prenotazioni
                .Include(p => p.Attivita)
                .Include(p => p.User)
                .Include(p => p.sportCentre).FirstOrDefaultAsync(p => p.Id == id);

            if (prenotazione == null)
            {
                return NotFound();
            }

            PrenotazioneVM.Id = prenotazione.Id;
            PrenotazioneVM.userId = prenotazione.userId;
            PrenotazioneVM.attivitaId = prenotazione.attivitaId;
            PrenotazioneVM.Data = prenotazione.Data.ToDateTime(new TimeOnly());
            PrenotazioneVM.sportCentreName = prenotazione.sportCentre.Name;
            PrenotazioneVM.sportCentreId = prenotazione.sportCentreId;

            await GetAttIdsUserIds();

            return Page();
        }

        //
        //___________________________________________________________________________________________
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    await GetAttIdsUserIds();
            //    return Page();
            //}

            var prenotazione = await _context.prenotazioni
                .FirstOrDefaultAsync(m => m.Id == PrenotazioneVM.Id);

            if (prenotazione == null)
            {
                return NotFound();
            }

            CheckAvailability(PrenotazioneVM.attivitaId, PrenotazioneVM.userId, DateOnly.FromDateTime(PrenotazioneVM.Data));

            if (!ModelState.IsValid)
            {
                await GetAttIdsUserIds();
                return Page();
            }

            prenotazione.userId = PrenotazioneVM.userId;
            prenotazione.attivitaId = PrenotazioneVM.attivitaId;
            prenotazione.Data = DateOnly.FromDateTime(PrenotazioneVM.Data);
            prenotazione.User = await _context.Users.FindAsync(PrenotazioneVM.userId);
            prenotazione.Attivita = await _context.attivita.FindAsync(PrenotazioneVM.attivitaId);
            prenotazione.sportCentreId = PrenotazioneVM.sportCentreId;
            //Prenotazione = prenotazione;

            _context.Attach(prenotazione).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                MessaggioConferma = "Prenotazione aggiornata con successo.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrenotazioneExists(prenotazione.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await GetAttIdsUserIds();

            return Page();
            //return RedirectToPage("./PrenotazioniIndex",new  { sportcentreid = PrenotazioneVM.sportCentreId });
        }

        //
        //___________________________________________________________________________________________
        public async Task GetAttIdsUserIds()
        {
            var allUsers = _userManager.Users.ToList();
            var usersWithRole = new List<IdentityUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("User"))
                {
                    usersWithRole.Add(user);
                }
            }

            ViewData["attivitaId"] = new SelectList(_context.attivita, "Id", "Name");
            ViewData["userId"] = new SelectList(usersWithRole, "Id", "UserName");
        }

        //
        //___________________________________________________________________________________________
        private bool PrenotazioneExists(int id)
        {
            return _context.prenotazioni.Any(e => e.Id == id); 
        }

        //
        //___________________________________________________________________________________________
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
