using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Models
{
    public class DataViewModel
    {
        public List<DataEntry> Entries { get; set; } = new List<DataEntry>();

        [BindProperty]
        [Required(ErrorMessage = "Text cannot be empty")]
        [Display(Name = "New Data Text")]
        public string? NewDataText { get; set; } 
    }
}
