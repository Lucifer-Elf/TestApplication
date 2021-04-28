using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servize.Domain.Model.VendorModel
{
    public class Review : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int VendorId { get; set; }
        [ForeignKey(nameof(VendorId))]
        public Vendor Vendor { get; set; }

        public int HappinessRating { get; set; }
        public string Product { get; set; }
        public string ReviewComment { get; set; }

    }
}
