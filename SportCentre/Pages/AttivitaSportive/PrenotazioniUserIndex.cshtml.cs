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

        public PaginatedList<Prenotazione>? Prenotazioni { get; set; }

        public PrenotazioniUserIndexModel(SportCentre.Data.ApplicationDbContext context,UserManager<IdentityUser> usermanager, IConfiguration configuration)
        {
            _context = context;
            _userManager = usermanager;
            Configuration = configuration;
        }

        //___________________________________________________________________________________________
        public async Task OnGetAsync(int? pageIndex)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) {
                return;
            }

            IQueryable<Prenotazione> prenotazioniIQ = _context.prenotazioni
                                    .Include(p => p.Attivita)
                                    .Include(p => p.User);

            var pageSize = Configuration.GetValue("PageSize", 4);

            Prenotazioni = await PaginatedList<Prenotazione>.CreateAsync(
                prenotazioniIQ.Where(p => p.User.Id == user.Id).AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
