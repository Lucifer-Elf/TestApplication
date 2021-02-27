using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Servize.Authentication;
using Servize.Domain.Model.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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

        public CartItem AddToCart(ServizeProduct category, int amount)
        {
            try
            {
                var cartItem = _context.CartItem.SingleOrDefault(s => s.ServizeSubCategory.Id == category.Id && s.CartId == Id);

                if (cartItem == null)
                {

                    cartItem = new CartItem
                    {
                        ProviderId = 1,
                        CartId = Id,
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


        public int  RemoveFromCart(ServizeProduct category, int amount)
        {
           
                var cartItem = _context.CartItem.SingleOrDefault(s => s.ServizeSubCategory.Id == category.Id && s.CartId == Id);

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
            return CartItems ?? (CartItems = _context.CartItem.Where(c => c.CartId == Id).Include(i => i.ServizeSubCategory).ToList());
        }


        public void ClearCart()
        {

            var cartItem = _context.CartItem.Where(c => c.CartId == Id);
            _context.CartItem.RemoveRange(cartItem);
            _context.SaveChanges();
        }


        public double GetCartTotal()
        {
            var total = _context.CartItem.Where(c => c.CartId == Id).Select(C => C.ServizeSubCategory.PriceQuote * C.Amount).Sum();
            return total;
        
        }
    }
}
