using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class DataEntry
    {
        [Key]
        public int Id {get; set;}

        public string TextData {get; set;} = string.Empty;
    }
}