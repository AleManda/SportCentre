using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public class PrenotazioniIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly IConfiguration Configuration;

        public string? CurrentFilterDate { get; set; }
        public string? CurrentFilterAttivita { get; set; }
        public string? CurrentFilterUser { get; set; }

        //[BindProperty]
        public int sportCentreId { get; set; }

        public string sportCentrName { get; set; }

        public PaginatedList<Prenotazione>? Prenotazioni { get; set; }

        public PrenotazioniIndexModel(SportCentre.Data.ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        //___________________________________________________________________________________________
        public async Task OnGetAsync(string searchdate,string searchuser,string searchattivita, int? pageIndex,int sportcentreid)
        {

            //ho già asp-for="CurrentFilterDate"(ma non bindproperty in model,qui) in pagina,questo serve?
            //il form fa get,non post,x questo non c'è binding?gli input del form vengono passati in routing
            //e non postati,quindi no binding?
            CurrentFilterDate = searchdate;
            CurrentFilterUser = searchuser;
            CurrentFilterAttivita = searchattivita;
            sportCentreId = sportcentreid;

            sportCentrName = _context.SportCentres
                .Where(s => s.id == sportcentreid)
                .Select(s => s.Name)
                .FirstOrDefault() ?? "Sport Centre";

            IQueryable<Prenotazione> prenotazioniIQ = _context.prenotazioni
                .Include(p => p.Attivita)
                .Include(p => p.User)
                .Include(p => p.sportCentre)
                .Where(p => p.sportCentreId == sportcentreid);
            
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

            var pageSize = Configuration.GetValue("PageSize", 11);

            Prenotazioni = await PaginatedList<Prenotazione>.CreateAsync(
                prenotazioniIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
