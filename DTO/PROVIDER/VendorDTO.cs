using System;
using System.Collections.Generic;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.DTO.PROVIDER
{
    public class VendorDTO
    {
        public VendorDTO()
        {
            Categories = new HashSet<CategoryDTO>();
        }

        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string EmiratesIdNumber { get; set; }

        public ServizeModeType ModeType { get; set; }

        public string Address { get; set; }  // Interact with google Api 

        public string Postal { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public string Certificate { get; set; }

        public int CovidRating { get; set; }

        public DateTime RegistrationDate { get; set; }

        public PackageType PackageType { get; set; }

        public DateTime Modified { get; set; }

        public ICollection<CategoryDTO> Categories { get; set; }
    }
}
