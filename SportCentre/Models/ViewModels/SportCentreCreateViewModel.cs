namespace SportCentre.Models.ViewModels
{
    public class SportCentreCreateViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }

        public List<int> SelectedAttivitaIds { get; set; } = new();
        public List<Attivita> AvailableAttivita { get; set; } = new();
    }

}
