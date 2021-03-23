using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servize.Domain.Enums;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Model.VendorModel;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Servize.Controllers
{
    //[Authorize(Roles = UserRoles.Client )]
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ServizeDBContext _context;
        private readonly Cart _cart;
        public CartController(ServizeDBContext dBContext, Cart cart)
        {
            _context = dBContext;
            _cart = cart;
        }


        [HttpGet]
        [Route("GetAll")]
        public ActionResult<Response<IList<CartItem>>> GetAllcart()
        {
            var value = _cart.GetCartItem();
            if (value.Count < 0)
                return new Response<IList<CartItem>>("NoData Avaliable in cart", StatusCodes.Status404NotFound);
            return new Response<IList<CartItem>>(value, StatusCodes.Status200OK);
        }


        [HttpPost]
        [Route("AddToCart")]
        public ActionResult<Response<CartItem>> AddtoCart([FromBody] CartDTO cartDTO)
        {
            var category = _context.Product.FirstOrDefault(p => p.Id == cartDTO.CategoryId);
            if (category != null)
            {
                var item = _cart.AddToCart(category, cartDTO.Amount);
                if (item == null)
                    return new Response<CartItem>("No data to add in Cart", StatusCodes.Status500InternalServerError);

                return new Response<CartItem>(item, StatusCodes.Status200OK);

            }
            return new Response<CartItem>("Category Id is not avaliable", StatusCodes.Status404NotFound);
        }

        [HttpDelete]
        [Route("RemoveFromCart")]
        public ActionResult<Response<Product>> RemoveFromCart(int productId, int amount)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _cart.RemoveFromCart(product, amount);
                return new Response<Product>(product, StatusCodes.Status200OK);
            }
            return new Response<Product>("Category Id is not avaliable", StatusCodes.Status404NotFound);

        }
    }
}
