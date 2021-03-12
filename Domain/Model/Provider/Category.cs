using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.Provider
{
    public class Category :BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Provider")]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }

        public Categories Type  { get; set; }

        public string BannerImage { get; set; }

        public ICollection<Product> SubServices { get; set; }

    }
}
