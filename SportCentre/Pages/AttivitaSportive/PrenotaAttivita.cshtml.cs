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

        public void OnGet(int id,string descr)
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
                ModelState.AddModelError(string.Empty, "Attività non trovata.");
                return Page();
            }

            int count = _context.prenotazioni.Count(p => p.Data == DateOnly.FromDateTime(Data)
                                                    && p.attivitaId == AttivitaSportiva.Id);
            if (count >= AttivitaSportiva.Posti)
            {
                ModelState.AddModelError(string.Empty, "Non ci sono più posti disponibili per questa data.");
                return Page();
            }

            AttivitaSportiva = _context.attivita.FirstOrDefault(a => a.Id == AttivitaSportiva.Id);
            if (AttivitaSportiva == null)
            {
                return NotFound();
            }
            if (Data < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "La data selezionata non è valida.");
                return Page();
            }
            //CheckAvailability();
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var prenotazione = new Prenotazione
            {
                userId = user.Id,
                attivitaId = AttivitaSportiva.Id,
                Data = DateOnly.FromDateTime(Data)
            };
            _context.prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();
            MessaggioConferma = $"Prenotazione effettuata con successo! {AttivitaSportiva.descrizione} in data {Data}";

            return Page();
        }
    }
}
