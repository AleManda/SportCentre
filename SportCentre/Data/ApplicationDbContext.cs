using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportCentre.Models;

namespace SportCentre.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Attivita> attivita { get; set; } = default!;
        public DbSet<Prenotazione> prenotazioni { get; set; } = default!;
        public DbSet<Centre> SportCentres { get; set; } = default!;
    }
}
