namespace SportCentre.Models
{
    public class PrenotazioneViewModel
    {

        public int Id { get; set; }
        public string? userId { get; set; }

        public int attivitaId { get; set; }

        public DateOnly Data { get; set; }
    }
}
