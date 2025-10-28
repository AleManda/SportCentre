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
    public class AttivitaIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;

        public AttivitaIndexModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Attivita> Attivita { get;set; } = default!;
        public IList<SportCentre.Models.SportCentre?> sportCentres { get; set; }

        public async Task OnGetAsync()
        {
            Attivita = await _context.attivita.ToListAsync();

            sportCentres = await _context.SportCentres.Include(sc => sc.sportCentreAttivita)
                .ThenInclude(sa => sa.Attivita)
                .ToListAsync();

        }
    }
}
