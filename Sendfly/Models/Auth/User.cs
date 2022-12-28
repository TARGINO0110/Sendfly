using System.ComponentModel.DataAnnotations;

namespace Sendfly.Models.Auth
{
    public class User
    {
        [Required(ErrorMessage = "Enter Username !")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Enter Password !")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Enter Role !")]
        public string Role { get; set; }
    }
}
