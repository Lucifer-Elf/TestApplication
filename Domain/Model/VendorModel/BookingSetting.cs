using System.ComponentModel.DataAnnotations.Schema;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Domain.Model.VendorModel
{
    public class BookingSetting : BaseEntity
    {
        public int Id { get; set; }

        public int VendorId { get; set; }

        [ForeignKey(nameof(VendorId))]
        public Vendor Vendor { get; set; }

        public bool BookingProcess { get; set; } // on off current state of acceptance 
        public int SlotsInterval { get; set; }
        public bool MyProperty { get; set; }
        public BookingAssignment BookingAssignment { get; set; }
        public bool AmountMountBasedOnService { get; set; }
        public string NextAvaliablity { get; set; }
    }
}
