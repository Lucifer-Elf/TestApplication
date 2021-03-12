using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.DTO.PROVIDER
{
    public class CategoryDTO
    {
        public CategoryDTO()
        {
            SubServices = new HashSet<ProductDTO>();
        }
        public int Id { get; set; }
      
        public int ProviderId { get; set; }
       
        public Categories Type { get; set; }

        public string BannerImage { get; set; }
        public DateTime Modified { get; set; }
        public ICollection<ProductDTO> SubServices { get; set; }
     
    }
}
