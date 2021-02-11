using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.DTO.PROVIDER
{
    public class ServizeReviewDTO
    {

        public int Rating { get; set; }

        public SubCategories SubCategory { get; set; }

        public string ReviewComment { get; set; }
    }
}
