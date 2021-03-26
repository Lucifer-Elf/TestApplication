using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO
{
    public class RegistrationInputModel :BaseInputModel
    {
        public string FirstName { get; set; }
        public string LastName  { get; set; }   
    }
}
