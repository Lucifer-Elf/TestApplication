using Servize.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model
{
    public class ServizeProvider
    {
        [Key]
        public int Id { get; set; }

        public ServizeModeType ModeType { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string Address { get; set; }  // Interact with google Api 

        public string Postal { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public string EmiratesIdNumber { get; set; }

        public bool PickAndDrop { get; set; }

        public int CovidRating { get; set; }

        public PackageType PackageType { get; set; }

        //public ICollection<ServizeCategory> ServiceCategories { get; set; }

        //public ICollection<ServizeReview> Reviews { get; set; }


    }
}
