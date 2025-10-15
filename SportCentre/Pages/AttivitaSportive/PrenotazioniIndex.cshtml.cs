using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SportCentre.Data;
using SportCentre.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SportCentre.Pages.AttivitaSportive
{
    public class PrenotazioniIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly IConfiguration Configuration;

        public string? CurrentFilterDate { get; set; }
        public string? CurrentFilterAttivita { get; set; }
        public string? CurrentFilterUser { get; set; }

        public PrenotazioniIndexModel(SportCentre.Data.ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        //public IList<Prenotazione> Prenotazioni { get;set; } = default!;

        public PaginatedList<Prenotazione>? Prenotazioni { get; set; }


        //___________________________________________________________________________________________
        public async Task OnGetAsync(string searchdate,string searchuser,string searchattivita, int? pageIndex)
        {
            CurrentFilterDate = searchdate;
            CurrentFilterUser = searchuser;
            CurrentFilterAttivita = searchattivita;

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

            var pageSize = Configuration.GetValue("PageSize", 4);

            Prenotazioni = await PaginatedList<Prenotazione>.CreateAsync(
                prenotazioniIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            //Prenotazioni = await prenotazioniIQ.ToListAsync();
            //Prenotazioni = await _context.prenotazioni
            //    .Include(p => p.Attivita)
            //    .Include(p => p.User).ToListAsync();
        }
    }
}
