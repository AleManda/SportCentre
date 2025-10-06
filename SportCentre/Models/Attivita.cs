namespace SportCentre.Models
{
    public class Attivita
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string descrizione { get; set; }
        public TimeSpan Orario { get; set; }

        public int postiDisponibili {  get; set; }
    }
}
