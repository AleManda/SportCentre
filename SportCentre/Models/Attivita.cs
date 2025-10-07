namespace SportCentre.Models
{
    public class Attivita
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? descrizione { get; set; }
        public int Orario { get; set; }

        public int Posti{  get; set; }

        public List<Prenotazione>? Prenotazioni { get; set; }




    }
}
