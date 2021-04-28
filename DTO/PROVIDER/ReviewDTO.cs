using System;

namespace Servize.DTO.PROVIDER
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        public int VendorId { get; set; }

        public int HappinessRating { get; set; }
        public string Product { get; set; }
        public string ReviewComment { get; set; }

        public DateTime Modified { get; set; }
    }
}
