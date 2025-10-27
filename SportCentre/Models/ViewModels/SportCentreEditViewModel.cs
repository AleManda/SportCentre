namespace SportCentre.Models.ViewModels
{
    public class SportCentreEditViewModel
    {
        public int id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;


        public List<int> existingAttivitaIds { get; set; } = new List<int>();

        public List<Attivita> AvailableAttivita { get; set; } = new();
    }
}
