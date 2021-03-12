using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO.PROVIDER
{
    public class CartDTO
    {
      
        public int Id { get; set; }
        public int ServizeCategoryNumber { get; set; }
        public int Amount { get; set; }
        public DateTime Modified { get; set; }
    }
}
