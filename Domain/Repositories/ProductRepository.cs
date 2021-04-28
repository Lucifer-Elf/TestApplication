using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model.VendorModel;
using Servize.Utility;
using Servize.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class ProductRepository : BaseRepository<ServizeDBContext>
    {
        private ServizeDBContext _context;

        public ProductRepository(ServizeDBContext dbcontext) : base(dbcontext)
        {
            this._context = dbcontext;
        }

        public async Task<Response<IList<Product>>> GetAllProductList()
        {
            try
            {
                List<Product> providerList = await _context.Product.AsNoTracking().ToListAsync();
                return new Response<IList<Product>>(providerList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<IList<Product>>($"Failed to get productList ", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<Product>> GetProductById(int id)
        {
            try
            {
                Product product = await _context.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
                if (product == null)
                    return new Response<Product>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<Product>(product, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<Product>($"Failed to get product Error", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
