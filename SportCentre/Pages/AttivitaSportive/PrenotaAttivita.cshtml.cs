using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportCentre.Models;

namespace SportCentre.Pages.AttivitaSportive
{
    public class PrenotaAttivitaModel : PageModel
    {
        [BindProperty]
        public DateTime Data { get; set; } = DateTime.MinValue;

        public Attivita? AttivitaSportiva { get; set; }

        private readonly SportCentre.Data.ApplicationDbContext _context;

        public PrenotaAttivitaModel(SportCentre.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet(int id,string descr)
        {
            Data = DateTime.Today.AddDays(1); 

            AttivitaSportiva = _context.attivita.FirstOrDefault(a => a.Id == id);
        }

        public void OnPost()
        {
            int count = _context.prenotazioni.Count(p => p.Data == DateOnly.FromDateTime(Data) && p.attivitaId == AttivitaSportiva.Id);
        }
    }
}
