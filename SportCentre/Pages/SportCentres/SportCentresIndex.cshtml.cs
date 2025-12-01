using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SportCentre.Data;
using SportCentre.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SportCentre.Pages.SportCentres
{
    [Authorize(Roles = "Admin")]
    public class SportCentresIndexModel : PageModel
    {
        private readonly SportCentre.Data.ApplicationDbContext _context;
        private readonly IConfiguration Configuration;

        public string? CurrentFilterName { get; set; }
        public string? CurrentFilterAddress { get; set; }
        public string? CurrentFilterLocation { get; set; }

        public SportCentresIndexModel(SportCentre.Data.ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        //public IList<SportCentre.Models.SportCentre> sportCentres { get; set; } = default!;


        public PaginatedList<Models.SportCentre>? sportCentres { get; set; }

        public async Task OnGetAsync(string searchname, string searchlocation, string searchaddress, int? pageIndex)
        {
            

            CurrentFilterName = searchname;
            CurrentFilterLocation = searchlocation;
            CurrentFilterAddress = searchaddress;
            
            IQueryable<Models.SportCentre> sportcentreIQ  =  _context.SportCentres;

            if (!string.IsNullOrEmpty(searchname))
            {
                sportcentreIQ = sportcentreIQ.Where(sc => sc.Name.Contains(searchname));
            }
            if (!string.IsNullOrEmpty(searchlocation))
            {
                sportcentreIQ = sportcentreIQ.Where(sc => sc.Location.Contains(searchlocation));
            }
            if (!string.IsNullOrEmpty(searchaddress))
            {
                sportcentreIQ = sportcentreIQ.Where(sc => sc.Address.Contains(searchaddress));
            }

            var pageSize = Configuration.GetValue("PageSize", 10);

            sportCentres = await PaginatedList<Models.SportCentre>.CreateAsync(
                sportcentreIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
