using System;
using System.Collections.Generic;

namespace Servize.DTO.PROVIDER
{
    public class CategoryDTO
    {
        public CategoryDTO()
        {
            Products = new HashSet<ProductDTO>();
        }
        public int Id { get; set; }

        public int VendorId { get; set; }

        public Categories Type { get; set; }

        public string BannerImage { get; set; }
        public DateTime Modified { get; set; }
        public ICollection<ProductDTO> Products { get; set; }

    }
}
