namespace SportCentre.Models
{
    public class Attivita
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Descrizione { get; set; }
        public int Orario { get; set; }

        public int Posti{  get; set; }

        public ICollection<Prenotazione>? Prenotazioni { get; set; } = new List<Prenotazione>();

        public ICollection<SportCentreAttivita>? sportCentreAttivita { get; set; } = new List<SportCentreAttivita>();






    }
}
