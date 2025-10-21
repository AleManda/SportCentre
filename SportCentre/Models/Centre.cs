using System.ComponentModel.DataAnnotations;
using SportCentre.Models;

namespace SportCentre.Models
{
    public class Centre
    {
   
        public int id { get; set; }
    
        public string Name { get; set; } = string.Empty;
     
        public string Address { get; set; } = string.Empty;
   
        public string Location { get; set; } = string.Empty;

        //public ICollection<Attivita>? AttivitaSportive { get; set; }
    }
}
