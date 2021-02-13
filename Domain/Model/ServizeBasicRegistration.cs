using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model
{
    public class ServizeBasicRegistration
    {
        
        /*[Required]
        [PersonalData]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        public string LastName { get; set; }

        [Required]
        [PersonalData]
        public string UserName { get; set; }*/

        [Required]
        [PersonalData]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The must be rule ", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password Doesnt Match")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

       /* [Required]
        [PersonalData]
        public int? PhoneNumber { get; set; }*/

    }
}
