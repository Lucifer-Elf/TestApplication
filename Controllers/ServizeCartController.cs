using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Model.Provider;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServizeCartController : ControllerBase
    {
        private readonly ServizeDBContext _context;
        private readonly Cart _cart;
        public ServizeCartController(ServizeDBContext dBContext,Cart cart)
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
       public ActionResult<Response<CartItem>> AddtoCart([FromBody]CartDTO cartDTO)
        {
            var category = _context.ServizeSubCategory.FirstOrDefault(p => p.Id == cartDTO.ServizeCategoryNumber);
            if (category != null)
            {
                var item = _cart.AddToCart(category, cartDTO.Amount);
                if(item==null)
                    return new Response<CartItem>("No data to add in Cart", StatusCodes.Status500InternalServerError);

                return new Response<CartItem>(item, StatusCodes.Status200OK);
                     
            }
            return new Response<CartItem>("Category Id is not avaliable", StatusCodes.Status404NotFound);
        }

        [HttpDelete]
        [Route("RemoveFromCart")]
        public ActionResult<Response<ServizeSubCategory>> RemoveFromCart(int subCategoryId, int amount)
        {
            var category = _context.ServizeSubCategory.FirstOrDefault(p => p.Id == subCategoryId);
            if (category != null)
            {
                _cart.RemoveFromCart(category, amount);
                return new Response<ServizeSubCategory>(category, StatusCodes.Status200OK);

            }

            return new Response<ServizeSubCategory>("Category Id is not avaliable", StatusCodes.Status404NotFound);

        }
    }
}
