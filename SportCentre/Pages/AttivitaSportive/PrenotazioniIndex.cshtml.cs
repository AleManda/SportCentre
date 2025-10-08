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

        public PrenotazioniIndexModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Prenotazione> Prenotazioni { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Prenotazioni = await _context.prenotazioni
                .Include(p => p.Attivita)
                .Include(p => p.User).ToListAsync();
        }
    }
}
