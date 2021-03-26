using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO
{
    public class RegistrationInputModelVendor:BaseInputModel
    {   

        public string CompanyName { get; set; }

        public string CompanyRegistrationNumber { get; set; }
    }
}
