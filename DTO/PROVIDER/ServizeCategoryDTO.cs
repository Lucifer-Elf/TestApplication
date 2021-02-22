using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.DTO.PROVIDER
{
    public class ServizeCategoryDTO
    {
        public ServizeCategoryDTO()
        {
            SubServices = new HashSet<ServizeSubCategoryDTO>();
        }
        public int Id { get; set; }
      
        public int ProviderId { get; set; }
       

        public Categories Type { get; set; }

        public string BannerImage { get; set; }

        public ICollection<ServizeSubCategoryDTO> SubServices { get; set; }
    }
}
