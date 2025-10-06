namespace SportCentre.Models
{
    public class Prenotazione
    {
        public string userId { get; set; }
        public int attivitaId { get; set; }

        public DateOnly attivitaDate { get; set; }

        public TimeSpan attivitaTime { get; set; }

    }
}
