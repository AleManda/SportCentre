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
        public DbSet<SportCentre.Models.SportCentre> SportCentres { get; set; } = default!;

        public DbSet<SportCentreAttivita> SportCentreAttivita { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SportCentreAttivita>()
                .HasKey(sa => new { sa.SportCentreId, sa.AttivitaId });

            modelBuilder.Entity<SportCentreAttivita>()
                .HasOne(sa => sa.SportCentre)
                .WithMany(sc => sc.sportCentreAttivita)
                .HasForeignKey(sa => sa.SportCentreId);

            modelBuilder.Entity<SportCentreAttivita>()
                .HasOne(sa => sa.Attivita)
                .WithMany(a => a.sportCentreAttivita)
                .HasForeignKey(sa => sa.AttivitaId);
        }

    }
}
