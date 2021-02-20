using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.Provider
{
    public class ServizeCategory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ServizeProvider")]
        public int ProviderId { get; set; }
        public ServizeProvider ServizeProvider { get; set; }

        public Categories Type  { get; set; }

        public string BannerImage { get; set; }

        public IEnumerable<ServizeSubCategory> SubServices { get; set; }

    }
}
