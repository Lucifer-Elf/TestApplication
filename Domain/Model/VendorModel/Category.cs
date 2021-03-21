using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.VendorModel
{
    public class Category :BaseEntity
    {
        [Key]
        public int Id { get; set; }
      
        public int VendorId { get; set; }
        [ForeignKey(nameof(VendorId))]
        public Vendor Vendor { get; set; }

        public Categories Type  { get; set; }

        public string BannerImage { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
