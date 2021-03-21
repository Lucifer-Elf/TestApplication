using Servize.Domain.Model.VendorModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servize.Domain.Model.OrderDetail
{
    public class CartItem : BaseEntity
    {
        public int Id { get; set; }


        public int VendorId { get; set; }

        public int CategoryId { get; set; }

        public Product Product { get; set; }

        public int Amount { get; set; }

        
        public string CartId { get; set; }
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }
    }
}
