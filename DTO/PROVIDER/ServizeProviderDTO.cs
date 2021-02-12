using Servize.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.DTO.PROVIDER
{
    public class ServizeProviderDTO
    {
        public ServizeProviderDTO()
        {
            ServiceCategories = new HashSet<ServizeCategoryDTO>();
            Reviews = new HashSet<ServizeReviewDTO>();
        }

        public string CompanyName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int? PhoneNumber { get; set; }

        public string Address { get; set; }  // Interact with google Api 

        public string Postal { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public string EmiratesIdNumber { get; set; }

        public bool PickAndDrop { get; set; }

        public PackageType PackageType { get; set; }

        public ICollection<ServizeCategoryDTO> ServiceCategories { get; set; }

        public ICollection<ServizeReviewDTO> Reviews { get; set; }
    }
}
