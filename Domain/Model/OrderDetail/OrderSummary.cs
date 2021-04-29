using Servize.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servize.Domain.Model.OrderDetail
{
    public class OrderSummary : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; }

        public double TotalAmount { get; set; }

    }
}
