using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model
{
    public class ServizeCategory
    {
        [Key]
        public int Id { get; set; }

        public Categories Type  { get; set; }

        public string BannerImage { get; set; }

        public IEnumerable<ServizeSubCategory> SubService { get; set; }

    }
}
