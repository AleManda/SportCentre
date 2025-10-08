using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportCentre.Models;
using System.Linq;

namespace SportCentre.Pages.AttivitaSportive
{
    public class PrenotaAttivitaModel : PageModel
    {
        [BindProperty]
        public int AttivitaId { get; set; }

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

        public void OnGet(int id)
        {
            Data = DateTime.Today.AddDays(1);
            AttivitaId = id;
            AttivitaSportiva = _context.attivita.FirstOrDefault(a => a.Id == id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AttivitaSportiva = await _context.attivita.FindAsync(AttivitaId);

            if (AttivitaSportiva == null)
            {
                ModelState.AddModelError(string.Empty, "Attivit� non trovata.");
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (Data < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "La data selezionata non � valida.");
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
                Data = DateOnly.FromDateTime(Data)
            };
            _context.prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();
            MessaggioConferma = $"Prenotazione effettuata con successo! {AttivitaSportiva.descrizione} in data {DateOnly.FromDateTime(Data)}";

            return Page();
        }

        private void CheckAvailability(string userid)
        {
            int count = _context.prenotazioni.Count(p => p.Data == DateOnly.FromDateTime(Data)
                                                    && p.attivitaId == AttivitaSportiva.Id);
            if (count >= AttivitaSportiva.Posti)
            {
                ModelState.AddModelError(string.Empty, "Non ci sono pi� posti disponibili per questa data.");
            }

            bool alreadyBooked = _context.prenotazioni.Any(p => p.Data == DateOnly.FromDateTime(Data)
                                                    && p.attivitaId == AttivitaSportiva.Id
                                                    && p.userId == userid);

            if (alreadyBooked)
            {
                ModelState.AddModelError(string.Empty, "Hai gi� una prenotazione per questa data.");
            }
        }
    }
}
