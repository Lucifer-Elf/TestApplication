using Servize.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO.USER
{
    public class ClientDTO
    {      
        public int Id { get; set; }
    
        public string UserId { get; set; }

        public string Address { get; set; }

        public string Area { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ProfilePicture { get; set; }
        public int OrderId { get; set; }
     
    }
}
