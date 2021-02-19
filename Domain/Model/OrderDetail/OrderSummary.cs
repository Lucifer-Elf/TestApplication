using Servize.Authentication;
using Servize.Domain.Model.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model.OrderDetail
{
    public class OrderSummary
    {
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string PromoCode { get; set; }

        [DefaultValue(5)]
        public int Vat { get; set; }

        public DateTime ServiceRequestDateTime { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
