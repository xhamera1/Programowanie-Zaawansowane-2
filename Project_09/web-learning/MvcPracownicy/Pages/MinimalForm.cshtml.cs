using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System; 

namespace MvcPracownicy.Pages
{
    public class MinimalForm : PageModel // Klasa PageModel
    {
        public string? mojtekst { get; set; }

        public void OnGet()
        {
            // Można tu coś zainicjalizować, jeśli potrzeba
            this.mojtekst = "Tekst inicjalizacyjny z OnGet.";

        }

       public void OnPost(IFormCollection form) // Zmieniona nazwa
        {
            this.mojtekst = string.Format("Mój tekst: {0}", form["mojtekst"].ToString());
        }
    }
}

// lub lepiej:
// public class MinimalForm : PageModel
// {
//     [BindProperty] // Bindowanie tej właściwości z danymi formularza POST
//     public string? MojtekstInput { get; set; } // Nazwa właściwości może odpowiadać atrybutowi 'name' inputa

//     public string? WyswietlanyTekst { get; set; }

//     public void OnGet() { }

//     public void OnPostSubmit() // Nie potrzebuje już IFormCollection
//     {
//         this.WyswietlanyTekst = $"Mój tekst: {MojtekstInput}";
//     }
// }