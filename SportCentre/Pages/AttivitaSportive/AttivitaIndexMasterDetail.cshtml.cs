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
    public class AttivitaIndexMasterDetailModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly IConfiguration Configuration;



        public string? CurrentFilterSportCentre { get; set; }
        public string? CurrentFilterAttività { get; set; }
        public string? CurrentFilterDescrizione { get; set; }
        public string? CurrentFilterOrario { get; set; }
        public string? CurrentFilterPosti { get; set; }
        public PaginatedList<SportCentre.Models.SportCentre>? sportcentres { get; set; }


        public AttivitaIndexMasterDetailModel(SportCentre.Data.ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IList<Attivita> Attivita { get; set; } = default!;


        //
        //_______________________________________________________________________________________________________
        public async Task<IActionResult> OnGetAsync(string searchsportcentre,string searchattivita,string searchdescrizione,
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


            //filtri di ricerca
            if (!string.IsNullOrEmpty(searchsportcentre))
            {
                sportcentresIQ = sportcentresIQ.Where(sp => sp.Name.Contains(searchsportcentre));
            }

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
                    sportcentresIQ = sportcentresIQ.Where(sc => sc.sportCentreAttivita.Any(sa => sa.Attivita.Posti == postiValue));
                }
            }

            //Paginazione,si passa la query preparata con i filtri
            var pageSize = Configuration.GetValue("PageSize", 11);
            sportcentres = await PaginatedList<SportCentre.Models.SportCentre>.CreateAsync(
                sportcentresIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            return Page();
        }


        //
        //_______________________________________________________________________________________________________
        public async Task<PartialViewResult> OnGetAttivitaDetailsPartialAsync(int scid)
        {
            //var scattivita = await _context.SportCentreAttivita
            //    .Include(sa => sa.Attivita)
            //    .Where(sa => sa.SportCentreId == scid).ToListAsync();

            var attivita = await _context.SportCentreAttivita
                                .Include(sa => sa.Attivita)
                                .Where(sa => sa.SportCentreId == scid)
                                .Select(sa => sa.Attivita)   // estrai solo le attività
                                .ToListAsync();


            return Partial("AttivitaDetailsPartial", attivita);
        }
    }
}
