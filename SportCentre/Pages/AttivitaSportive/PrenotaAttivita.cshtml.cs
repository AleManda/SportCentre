using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SportCentre.Models;
using System.Linq;

namespace SportCentre.Pages.AttivitaSportive
{
    public class PrenotaAttivitaModel : PageModel
    {
        [BindProperty]
        public int AttivitaId { get; set; }

        [BindProperty]
        public int SportCentreId { get; set; }

        public string SportCentreName { get; set; } = string.Empty;

        [BindProperty]
        public DateTime Data { get; set; } = DateTime.MinValue;

        public Attivita? AttivitaSportiva { get; set; }

        public string? MessaggioConferma { get; set; }

        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PrenotaAttivitaModel(SportCentre.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int attivitaid,int sportcentreid)
        {
            Data = DateTime.Today.AddDays(1);
            AttivitaId = attivitaid;
            SportCentreId = sportcentreid;
            SportCentreName = (await _context.SportCentres.FindAsync(sportcentreid))?.Name ?? "Centro Sportivo";
            AttivitaSportiva = await _context.attivita.FirstOrDefaultAsync(a => a.Id == attivitaid);
            return Page();                            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AttivitaSportiva = await _context.attivita.FindAsync(AttivitaId);

            if (AttivitaSportiva == null)
            {
                ModelState.AddModelError(string.Empty, "Attività non trovata.");
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (Data < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "La data selezionata non è valida.");
                return Page();
            }
            CheckAvailability(user.Id);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var prenotazione = new Prenotazione
            {
                userId = user.Id,
                attivitaId = AttivitaSportiva.Id,
                sportCentreId = SportCentreId,
                Data = DateOnly.FromDateTime(Data)
            };
            _context.prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();
            MessaggioConferma = $"Prenotazione effettuata con successo! {AttivitaSportiva.descrizione} presso {SportCentreName} in data {DateOnly.FromDateTime(Data)}";

            return Page();
        }

        private void CheckAvailability(string userid)
        {
            int count = _context.prenotazioni.Count(p => p.Data == DateOnly.FromDateTime(Data)
                                                    && p.attivitaId == AttivitaSportiva.Id);
            if (count >= AttivitaSportiva.Posti)
            {
                ModelState.AddModelError(string.Empty, "Non ci sono più posti disponibili per questa data.");
            }

            bool alreadyBooked = _context.prenotazioni.Any(p => p.Data == DateOnly.FromDateTime(Data)
                                                    && p.attivitaId == AttivitaSportiva.Id
                                                    && p.userId == userid);

            if (alreadyBooked)
            {
                ModelState.AddModelError(string.Empty, "Hai già una prenotazione per questa data.");
            }
        }
    }
}
