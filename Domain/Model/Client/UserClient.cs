using Servize.Authentication;
using Servize.Domain.Model.OrderDetail;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servize.Domain.Model.Client
{
    public class UserClient
    {

        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string Address { get; set; }
        
        public string Area { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ProfilePicture { get; set; }


    }
}
