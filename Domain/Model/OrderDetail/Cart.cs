using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Servize.Authentication;
using Servize.Domain.Model.VendorModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Servize.Domain.Model.OrderDetail
{
    public class Cart
    {
        public string Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public List<CartItem> CartItems { get; set; }





        private readonly ServizeDBContext _context;
        public Cart(ServizeDBContext context)
        {
            _context = context;
        }


        public static Cart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            var context = services.GetService<ServizeDBContext>();
            Console.WriteLine(session.GetString("CardId"));
            string cartId = session.GetString("CardId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new Cart(context) { Id = cartId };

        }

        public CartItem AddToCart(Product category, int amount)
        {
            try
            {
                var cartItem = _context.CartItem.SingleOrDefault(s => s.Product.Id == category.Id && s.CartId == Id);

                if (cartItem == null)
                {

                    cartItem = new CartItem
                    {
                        VendorId = 1,
                        CartId = Id,
                        Product = category,
                        Amount = amount
                    };

                    _context.CartItem.Add(cartItem);
                }
                else
                {

                    cartItem.Amount++;
                }
                _context.SaveChanges();
                return cartItem;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "While addtocart");
                return null;

            }


        }


        public int RemoveFromCart(Product product, int amount)
        {

            var cartItem = _context.CartItem.SingleOrDefault(s => s.Product.Id == product.Id && s.CartId == Id);

            var localAmount = 0;
            if (cartItem == null)
            {
                if (cartItem.Amount > 1)
                {
                    cartItem.Amount--;
                    localAmount = cartItem.Amount;
                }
                else
                {
                    _context.CartItem.Remove(cartItem);
                }
            }

            _context.SaveChanges();
            return localAmount;

        }

        public List<CartItem> GetCartItem()
        {
            return CartItems ?? (CartItems = _context.CartItem.Where(c => c.CartId == Id).Include(i => i.Product).ToList());
        }


        public void ClearCart()
        {

            var cartItem = _context.CartItem.Where(c => c.CartId == Id);
            _context.CartItem.RemoveRange(cartItem);
            _context.SaveChanges();
        }


        public double GetCartTotal()
        {
            var total = _context.CartItem.Where(c => c.CartId == Id).Select(C => C.Product.PriceQuote * C.Amount).Sum();
            return total;

        }
    }
}
