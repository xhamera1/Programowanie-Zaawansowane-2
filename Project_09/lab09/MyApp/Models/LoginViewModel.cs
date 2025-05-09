using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Login cannot be empty")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

    }
}