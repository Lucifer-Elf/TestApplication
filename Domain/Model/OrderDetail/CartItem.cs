using Servize.Domain.Model.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model.OrderDetail
{
    public class CartItem : BaseEntity
    {
        public int Id { get; set; }

        public int ProviderId { get; set; }

        public int ServizeCategoryId { get; set; }

        public Product ServizeProduct { get; set; }

        public int Amount { get; set; }

        [ForeignKey("Cart")]
        public string CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
