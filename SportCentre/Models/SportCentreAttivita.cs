namespace SportCentre.Models
{
    public class SportCentreAttivita
    {
        public int SportCentreId { get; set; }
        public SportCentre SportCentre { get; set; }

        public int AttivitaId { get; set; }
        public Attivita Attivita { get; set; }

        public DateTime CreatedAt { get; set; } 
    }

}
