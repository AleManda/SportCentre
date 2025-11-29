using System.ComponentModel.DataAnnotations;
using SportCentre.Models;

namespace SportCentre.Models
{
    public class SportCentre
    {
   
        public int id { get; set; }
    
        public string Name { get; set; } = string.Empty;
     
        public string Address { get; set; } = string.Empty;
   
        public string Location { get; set; } = string.Empty;

        public List<SportCentreAttivita>? sportCentreAttivita { get; set; } = new List<SportCentreAttivita>();

        //public ICollection<Prenotazione> prenotazioni { get; set;} = new List<Prenotazione>();
    }
}
