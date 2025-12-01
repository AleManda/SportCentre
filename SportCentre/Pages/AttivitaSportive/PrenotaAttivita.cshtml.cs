using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SportCentre.Models;
using SportCentre.Models.ViewModels;
using System.Linq;

namespace SportCentre.Pages.AttivitaSportive
{
    [Authorize(Roles = "User,Admin")]
    public class PrenotaAttivitaModel : PageModel
    {

        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public PrenotazioneViewModel PrenotazioneViewModel { get; set; } = new PrenotazioneViewModel();

        public string? MessaggioConferma { get; set; }



        public PrenotaAttivitaModel(SportCentre.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //
        //___________________________________________________________________________________________
        public async Task<IActionResult> OnGetAsync(int attivitaid,int sportcentreid)
        {
            PrenotazioneViewModel.Data = DateTime.Today.AddDays(1);
            PrenotazioneViewModel.attivitaId = attivitaid;
            PrenotazioneViewModel.sportCentreId = sportcentreid;
            //SportCentreName = (await _context.SportCentres.FindAsync(sportcentreid))?.Name ?? "Centro Sportivo";
            PrenotazioneViewModel.AttivitaSportiva = await _context.attivita.FirstOrDefaultAsync(a => a.Id == attivitaid);
            return Page();                            
        }

        //
        //___________________________________________________________________________________________
        public async Task<IActionResult> OnPostAsync()
        {
            PrenotazioneViewModel.AttivitaSportiva = await _context.attivita.FindAsync(PrenotazioneViewModel.attivitaId);

            if (PrenotazioneViewModel.AttivitaSportiva == null)
            {
                ModelState.AddModelError(string.Empty, "Attività non trovata.");
                return Page();
            }

            if (PrenotazioneViewModel.Data < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "La data selezionata non è valida.");
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            CheckAvailability(user.Id, PrenotazioneViewModel.AttivitaSportiva);



            if (!ModelState.IsValid)
            {
                return Page();
            }

            var prenotazione = new Prenotazione
            {
                userId = user.Id,
                attivitaId = PrenotazioneViewModel.AttivitaSportiva.Id,
                sportCentreId = PrenotazioneViewModel.sportCentreId,
                Data = DateOnly.FromDateTime(PrenotazioneViewModel.Data)
            };
            _context.prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();
            MessaggioConferma = $"Prenotazione effettuata con successo! {PrenotazioneViewModel.AttivitaSportiva.Descrizione} presso {PrenotazioneViewModel.sportCentreName} in data {DateOnly.FromDateTime(PrenotazioneViewModel.Data)}";

            return Page();
        }

        private void CheckAvailability(string userid, Attivita AttivitaSportiva)
        {
            int count = _context.prenotazioni.Count(p => p.Data == DateOnly.FromDateTime(PrenotazioneViewModel.Data)
                                                    && p.attivitaId == AttivitaSportiva.Id);
            if (count >= AttivitaSportiva.Posti)
            {
                ModelState.AddModelError(string.Empty, "Non ci sono più posti disponibili per questa data.");
            }

            bool alreadyBooked = _context.prenotazioni.Any(p => p.Data == DateOnly.FromDateTime(PrenotazioneViewModel.Data)
                                                    && p.attivitaId == AttivitaSportiva.Id
                                                    && p.userId == userid);

            if (alreadyBooked)
            {
                ModelState.AddModelError(string.Empty, "Hai già una prenotazione per questa data.");
            }
        }
    }
}
