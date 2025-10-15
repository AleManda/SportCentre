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
    public class PrenotazioniIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;

        [BindProperty]
        public string CurrentFilterDate { get; set; }
        public string CurrentFilterAttivita { get; set; }
        public string CurrentFilterUser { get; set; }

        public PrenotazioniIndexModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Prenotazione> Prenotazioni { get;set; } = default!;

        public async Task OnGetAsync(string searchdate,string searchuser,string searchattivita)
        {
            IQueryable<Prenotazione> prenotazioniIQ = _context.prenotazioni
                .Include(p => p.Attivita)
                .Include(p => p.User);

            if(!string.IsNullOrEmpty(searchdate) && DateOnly.TryParse(searchdate, out var parsedDate))
            {
                prenotazioniIQ = prenotazioniIQ.Where(p => p.Data == parsedDate);
            }
            if (!string.IsNullOrEmpty(searchuser))
            {
                prenotazioniIQ = prenotazioniIQ.Where(p => p.User.UserName.Contains(searchuser));
            }
            if (!string.IsNullOrEmpty(searchattivita))
            {
                prenotazioniIQ = prenotazioniIQ.Where(p => p.Attivita.Name.Contains(searchattivita));
            }

            Prenotazioni = await prenotazioniIQ.ToListAsync();
            //Prenotazioni = await _context.prenotazioni
            //    .Include(p => p.Attivita)
            //    .Include(p => p.User).ToListAsync();
        }
    }
}
