using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO
{
    public class RegistrationInputModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }   
        public string Password { get; set; }     
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        public string LastName  { get; set; }        

        public string CompanyName { get; set; }
        public string CompanyRegistrationNumber { get; set; }

        public int Otp { get; set; }
        public string Role { get; set; }

        public bool RememberMe { get; set; }

        

    }
}
