using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SportCentre.Data;
using SportCentre.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SportCentre.Pages.AttivitaSportive
{
    [Authorize(Roles = "User,Admin")]
    public class AttivitaIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly IConfiguration Configuration;



        public string? CurrentFilterSportCentre { get; set; }
        public string? CurrentFilterAttività { get; set; }
        public string? CurrentFilterDescrizione { get; set; }
        public string? CurrentFilterOrario { get; set; }
        public string? CurrentFilterPosti { get; set; }
        public PaginatedList<SportCentre.Models.SportCentre>? sportcentres { get; set; }


        public AttivitaIndexModel(SportCentre.Data.ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IList<Attivita> Attivita { get; set; } = default!;

        public async Task OnGetAsync(string searchsportcentre,string searchattivita,string searchdescrizione,
                                      string searchorario,string searchposti,int? pageIndex)
        {
            CurrentFilterSportCentre = searchsportcentre;
            CurrentFilterAttività = searchattivita;
            CurrentFilterDescrizione = searchdescrizione;
            CurrentFilterOrario = searchorario;
            CurrentFilterPosti = searchposti;




            IQueryable <SportCentre.Models.SportCentre> sportcentresIQ  = _context.SportCentres 
                                                                          .Include(sc => sc.sportCentreAttivita)
                                                                          .ThenInclude(sa => sa.Attivita);

            if (!string.IsNullOrEmpty(searchattivita))
            {
                sportcentresIQ = sportcentresIQ.Where(sc => sc.sportCentreAttivita.Any(sa => sa.Attivita.Name.Contains(searchattivita)));
            }

            if (!string.IsNullOrEmpty(searchdescrizione))
            {
                sportcentresIQ = sportcentresIQ.Where(sc => sc.sportCentreAttivita.Any(sa => sa.Attivita.Descrizione.Contains(searchdescrizione)));
            }

            if (!string.IsNullOrEmpty(searchorario))
            {
                if (int.TryParse(searchorario, out int orarioValue))
                {
                    sportcentresIQ = sportcentresIQ.Where(sc => sc.sportCentreAttivita.Any(sa => sa.Attivita.Orario == orarioValue));
                }
            }

            if (!string.IsNullOrEmpty(searchposti))
            {
                if (int.TryParse(searchposti, out int postiValue))
                {
                    sportcentresIQ = sportcentresIQ.Where(sc => sc.sportCentreAttivita.Any(sa => sa.Attivita.Posti >= postiValue));
                }
            }

            var pageSize = Configuration.GetValue("PageSize", 11);

            sportcentres = await PaginatedList<SportCentre.Models.SportCentre>.CreateAsync(
                sportcentresIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
