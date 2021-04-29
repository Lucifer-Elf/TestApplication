using Servize.Domain.Model.OrderDetail;
using System;
using System.Collections.Generic;

namespace Servize.DTO
{
    public class OrderSummaryDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IEnumerable<OrderItemDTO> OrderItems { get; set; }
        public double TotalAmount { get; set; }
        public DateTime Modified { get; set; }
    }
}
