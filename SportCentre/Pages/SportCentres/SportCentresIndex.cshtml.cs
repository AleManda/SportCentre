using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SportCentre.Data;
using SportCentre.Models;

namespace SportCentre.Pages.SportCentres
{
    public class SportCentresIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;

        public SportCentresIndexModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<SportCentre.Models.SportCentre> sportCentres { get;set; } = default!;

        public async Task OnGetAsync()
        {
            sportCentres = await _context.SportCentres.ToListAsync();
        }
    }
}
