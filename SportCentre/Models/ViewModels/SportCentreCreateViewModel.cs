using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SportCentre.Models;

namespace SportCentre.Models.ViewModels
{
    public class SportCentreCreateViewModel : IValidatableObject
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public List<int> SelectedAttivitaIds { get; set; } = new();
        public List<Attivita> AvailableAttivita { get; set; } = new();


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SelectedAttivitaIds == null || SelectedAttivitaIds.Count == 0)
                yield return new ValidationResult("Seleziona almeno un'attività.", new[] { nameof(SelectedAttivitaIds) });

            if (Name?.Length > 50)
                yield return new ValidationResult("Nome troppo lungo.", new[] { nameof(Name) });

            if (Address?.Length > 50)
                yield return new ValidationResult("Address troppo lungo.", new[] { nameof(Address) });

            if (Location?.Length > 50)
                yield return new ValidationResult("Location troppo lungo.", new[] { nameof(Location) });
        }
    }
}
