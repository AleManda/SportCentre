using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SportCentre.Data;
using SportCentre.Models;

namespace SportCentre.Pages.AttivitaSportive
{
    public class PrenotazioniUserIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PrenotazioniUserIndexModel(SportCentre.Data.ApplicationDbContext context,UserManager<IdentityUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        public IList<Prenotazione> Prenotazioni { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) {
                return;
            }

            Prenotazioni = await _context.prenotazioni
                .Include(p => p.Attivita)
                .Include(p => p.User).Where(p => p.User.Id == user.Id).ToListAsync();
        }
    }
}
