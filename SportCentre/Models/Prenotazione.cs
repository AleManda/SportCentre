using Microsoft.AspNetCore.Identity;

namespace SportCentre.Models
{
    public class Prenotazione
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public int attivitaId { get; set; }

        public DateOnly Data { get; set; }

        public string UserId { get; set; }

        public IdentityUser Utente { get; set; }

        public int AttivitaId { get; set; }

        public Attivita Attivita { get; set; }



    }
}
