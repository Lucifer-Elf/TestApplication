using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model.Provider
{
    public class ServizeProviderBankDetail
    {
        public int Id { get; set; }

        [ForeignKey("ServizeProvider")]
        public int ProviderId { get; set; }
        public ServizeProvider ServizeProvider { get; set; }

        public string AccountHolderName { get; set; }
        public Double AccountNumber { get; set; }
        public string SwiftCode { get; set; }
        public string BankName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
