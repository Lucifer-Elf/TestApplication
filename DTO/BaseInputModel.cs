using System.ComponentModel.DataAnnotations;

namespace Servize.DTO
{
    public class BaseInputModel
    {

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }

        public int Otp { get; set; }
        public string Role { get; set; }
    }
}
