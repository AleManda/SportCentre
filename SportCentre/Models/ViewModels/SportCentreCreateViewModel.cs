using Microsoft.Build.Framework;

namespace SportCentre.Models.ViewModels
{
    public class SportCentreCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Location { get; set; }

        public List<int> SelectedAttivitaIds { get; set; } = new();
        public List<Attivita> AvailableAttivita { get; set; } = new();
    }

}
