using Microsoft.AspNetCore.Identity;

namespace SportCentre.Models
{
    public class Prenotazione
    {
        public int Id { get; set; }
        public string? userId { get; set; }
        public int attivitaId { get; set; }

        public int sportCentreId { get; set; }

        public DateOnly Data { get; set; }




        public IdentityUser User { get; set; }

        public Attivita Attivita { get; set; }

        public SportCentre sportCentre { get; set; }



    }
}
