using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.Provider
{
    public class ServizeCategory
    {
        [Key]
        public int Id { get; set; }

        public Categories Type  { get; set; }

        public string BannerImage { get; set; }

        public bool PickAndDrop { get; set; }

        public IEnumerable<ServizeSubCategory> SubService { get; set; }

    }
}
