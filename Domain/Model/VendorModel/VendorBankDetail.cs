using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model.VendorModel
{
    public class VendorBankDetail : BaseEntity
    {
        public int Id { get; set; }

        public int VendorId { get; set; }
        [ForeignKey(nameof(VendorId))]
        public Vendor Vendor { get; set; }

        public string AccountHolderName { get; set; }
        public Double AccountNumber { get; set; }
        public string SwiftCode { get; set; }
        public string BankName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
