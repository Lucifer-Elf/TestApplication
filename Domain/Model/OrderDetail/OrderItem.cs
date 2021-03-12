
using Servize.Domain.Model.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servize.Domain.Model.OrderDetail
{
    public class OrderItem : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderNumber { get; set; }
        public OrderSummary Order { get; set; }

        [ForeignKey("ServizeProvider")]
        public int ProviderId { get; set; }
        public Provider.Provider ServizeProvider { get; set; }

        public double ItemDiscount { get; set; }

        public DateTime OrderDateTimne { get; set; }

        [DefaultValue(5)]
        public int Vat { get; set; }
        public double Amount { get; set; }


       

    }
}
