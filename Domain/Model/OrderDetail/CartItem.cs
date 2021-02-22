using Servize.Domain.Model.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model.OrderDetail
{
    public class CartItem
    {
        public int Id { get; set; }

        public int ProviderId { get; set; }

        public int ServizeCategoryId { get; set; }

        public ServizeSubCategory ServizeSubCategory { get; set; }

        public int Amount { get; set; }
        public string CartId { get; set; }
    }
}
