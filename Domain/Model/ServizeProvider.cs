using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model
{
    public class ServizeProvider
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int? PhoneNumber { get; set; }

        public string Address { get; set; }  // Interact with google Api 

        public string Postal { get; set; }

        public PackageType PackageType { get; set; }

        public ICollection<ServizeCategory> ServiceCategories { get; set; }

        public ICollection<ServizeReview> Reviews { get; set; }


    }
}
