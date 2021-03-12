using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.DTO.PROVIDER
{
    public class ReviewDTO
    {

        public int Rating { get; set; }

        public string Product { get; set; }

        public string ReviewComment { get; set; }

        public DateTime Modified { get; set; }
    }
}
