namespace SportCentre.Models.ViewModels
{
    public class PrenotazioneViewModel
    {

        public int Id { get; set; }
        public string? userId { get; set; }
        public string? userName { get; set; }

        public int attivitaId { get; set; }

        public DateTime Data { get; set; }

        public string sportCentreName { get; set; } = string.Empty; 

        public int sportCentreId { get; set; }

        public Attivita? AttivitaSportiva { get; set; }
    }
}
