using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Servize.Domain.Model.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Model.OrderDetail
{
    public class Cart
    {
        public string CartId { get; set; }
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

            return new Cart(context) { CartId = cartId };
          
        }

        public CartItem AddToCart(ServizeSubCategory category, int amount)
        {
            try
            {
                var cartItem = _context.CartItem.SingleOrDefault(s => s.ServizeSubCategory.Id == category.Id && s.CartId == CartId);

                if (cartItem == null)
                {

                    cartItem = new CartItem
                    {
                        ProviderId = 1,
                        CartId = CartId,
                        ServizeSubCategory = category,
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


        public int  RemoveFromCart(ServizeSubCategory category, int amount)
        {
           
                var cartItem = _context.CartItem.SingleOrDefault(s => s.ServizeSubCategory.Id == category.Id && s.CartId == CartId);

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
            return CartItems ?? (CartItems = _context.CartItem.Where(c => c.CartId == CartId).Include(i => i.ServizeSubCategory).ToList());
        }


        public void ClearCart()
        {

            var cartItem = _context.CartItem.Where(c => c.CartId == CartId);
            _context.CartItem.RemoveRange(cartItem);
            _context.SaveChanges();
        }


        public double GetCartTotal()
        {
            var total = _context.CartItem.Where(c => c.CartId == CartId).Select(C => C.ServizeSubCategory.PriceQuote * C.Amount).Sum();
            return total;
        
        }
    }
}
