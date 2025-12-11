using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
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
    public class AttivitaIndexMasterDetail2Model : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly IConfiguration Configuration;


        public string? CurrentFilterAttività { get; set; }
        public string? CurrentFilterDescrizione { get; set; }
        public string? CurrentFilterOrario { get; set; }
        public string? CurrentFilterPosti { get; set; }
        public List<SportCentre.Models.SportCentre>? sportcentres { get; set; }


        public AttivitaIndexMasterDetail2Model(SportCentre.Data.ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IList<Attivita> Attivita { get; set; } = default!;


        //
        //_______________________________________________________________________________________________________
        public async Task<IActionResult> OnGetAsync( string searchattivita, string searchdescrizione,
                                      string searchorario, string searchposti)
        {

            CurrentFilterAttività = searchattivita;
            CurrentFilterDescrizione = searchdescrizione;
            CurrentFilterOrario = searchorario;
            CurrentFilterPosti = searchposti;

            IQueryable<SportCentre.Models.SportCentre> sportcentresIQ = _context.SportCentres
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

            sportcentres = await sportcentresIQ.ToListAsync();

            return Page();
        }

        //___________________________________________________________________________________________
        // Endpoint AJAX per DataTables child rows,pagina fa chiamata ajax qui
        public async Task<JsonResult> OnGetGetAttivitaAsync(string sportcentreName,string searchattivita,string searchdescrizione,
            string searchOrario,string searchPosti)
        {
            List<Attivita> attivita;

            IQueryable<Attivita> attivitaIQ = _context.SportCentreAttivita
                .Include(sa => sa.Attivita)
                .Where(sa => sa.SportCentre.Name == sportcentreName).
                Select(sa => new Attivita
                {
                    Name = sa.Attivita.Name,
                    Descrizione = sa.Attivita.Descrizione,
                    Orario = sa.Attivita.Orario,
                    Posti = sa.Attivita.Posti
                });
                //.Select(sa => new
                //{
                //    name = sa.Attivita.Name,
                //    descrizione = sa.Attivita.Descrizione,
                //    orario = sa.Attivita.Orario,
                //    posti = sa.Attivita.Posti
                //});
               

            if (!string.IsNullOrEmpty(searchattivita))
            {
                attivitaIQ = attivitaIQ.Where(sa => sa.Name.Contains(searchattivita));
            }
            if (!string.IsNullOrEmpty(searchdescrizione))
            {
                attivitaIQ = attivitaIQ.Where(sa => sa.Descrizione.Contains(searchdescrizione));
            }
            if (!string.IsNullOrEmpty(searchOrario))
            { 
                if (int.TryParse(searchOrario, out int orarioValue))
                {
                    attivitaIQ = attivitaIQ.Where(sa => sa.Orario == orarioValue);
                }
            }
            if (!string.IsNullOrEmpty(searchPosti))
            {
                if (int.TryParse(searchPosti, out int postiValue))
                {
                    attivitaIQ = attivitaIQ.Where(sa => sa.Posti >= postiValue);
                }
            }

            attivita = await attivitaIQ.ToListAsync();


            return new JsonResult(attivita);
        }


    }
}
