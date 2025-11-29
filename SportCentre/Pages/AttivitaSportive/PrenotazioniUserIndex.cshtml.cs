using Microsoft.AspNetCore.Identity;
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
    public class PrenotazioniUserIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration Configuration;

        public string? CurrentFilterSportCentre { get; set; }
        public string? CurrentFilterDate { get; set; }
        public string? CurrentFilterAttivita { get; set; }
        public string? CurrentFilterAttivitaDescr { get; set; }

        public PaginatedList<Prenotazione>? Prenotazioni { get; set; }

        public PrenotazioniUserIndexModel(SportCentre.Data.ApplicationDbContext context,UserManager<IdentityUser> usermanager, IConfiguration configuration)
        {
            _context = context;
            _userManager = usermanager;
            Configuration = configuration;
        }

        //___________________________________________________________________________________________
        public async Task OnGetAsync(string searchdate, string searchsportcentre, string searchattivita, string searchattivitadescr, int? pageIndex)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) {
                return;
            }

            CurrentFilterSportCentre = searchsportcentre;
            CurrentFilterDate = searchdate;
            CurrentFilterAttivita = searchattivita;
            CurrentFilterAttivitaDescr = searchattivitadescr;


            IQueryable<Prenotazione> prenotazioniIQ = _context.prenotazioni
                                    .Include(p => p.Attivita)
                                    .Include(p => p.User)
                                    .Include(p => p.sportCentre);

            if (!string.IsNullOrEmpty(searchdate) && DateOnly.TryParse(searchdate, out var parsedDate))
            {
                prenotazioniIQ = prenotazioniIQ.Where(p => p.Data == parsedDate);
            }
            if (!string.IsNullOrEmpty(searchsportcentre))
            {
                prenotazioniIQ = prenotazioniIQ.Where(p => p.sportCentre.Name.Contains(searchsportcentre));
            }
            if (!string.IsNullOrEmpty(searchattivita))
            {
                prenotazioniIQ = prenotazioniIQ.Where(p => p.Attivita.Name.Contains(searchattivita));
            }
            if (!string.IsNullOrEmpty(searchattivitadescr))
            {
                prenotazioniIQ = prenotazioniIQ.Where(p => p.Attivita.Descrizione.Contains(searchattivitadescr));
            }

            var pageSize = Configuration.GetValue("PageSize", 4);

            Prenotazioni = await PaginatedList<Prenotazione>.CreateAsync(
                prenotazioniIQ.Where(p => p.User.Id == user.Id).AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
