using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class UserLogin
    {
        [Key]
        public int Id {get; set;}

        [Required(ErrorMessage ="Login cannot be empty")]
        public string Username {get; set;} = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty")]
        public string PasswordHash {get; set;} = string.Empty;
    }
}