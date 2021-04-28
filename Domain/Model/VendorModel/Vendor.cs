using Servize.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.VendorModel
{
    public class Vendor : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CompanyRegistrationNumber { get; set; }

        public ServizeModeType ModeType { get; set; }

        public string Address { get; set; }  // Interact with google Api 

        public string Postal { get; set; }

        public string Certificate { get; set; }

        public int CovidRating { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public PackageType PackageType { get; set; }

        public ICollection<Category> Categories { get; set; }


    }
}
