using System;

namespace Servize.Domain.Model.OrderDetail
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int VendorId { get; set; }

        public int Quantity { get; set; }
        public double PricePerItem { get; set; }

        public double ItemDiscount { get; set; }
        public DateTime OrderDateTimne { get; set; }
        public DateTime OrderPlacedDate { get; set; }
        public int Vat { get; set; }
        public double Amount { get; set; }
    }
}
