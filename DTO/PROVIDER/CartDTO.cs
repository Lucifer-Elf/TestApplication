using System;

namespace Servize.DTO.PROVIDER
{
    public class CartDTO
    {

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Amount { get; set; }
        public DateTime Modified { get; set; }
    }
}
