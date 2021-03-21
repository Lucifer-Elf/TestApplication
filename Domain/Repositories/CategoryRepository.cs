using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model.VendorModel;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class CategoryRepository
    {
        private readonly ServizeDBContext _context;
        public CategoryRepository(ServizeDBContext dBContext)
        {
            _context = dBContext;
        }

        public async Task<Response<IList<Category>>> GetAllCategoryList()
        {
            try
            {
                List<Category> servizeProviderList = await _context.Category.Include(i => i.Products).ToListAsync();
                return new Response<IList<Category>>(servizeProviderList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<IList<Category>>($"Failed to get ServiceCategoryList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<Response<Category>> GetAllCategoryById(int Id)
        {
            try
            {
                Category servizeProvider = await _context.Category.Include(i => i.Products)
                                                                                 .AsNoTracking()
                                                                                .SingleOrDefaultAsync(c => c.Id == Id);
                if (servizeProvider == null)
                    return new Response<Category>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<Category>(servizeProvider, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<Category>($"Failed to get ServiceCategory Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<Category>> PostCategory(Category category)
        {
            if (category == null)
                return new Response<Category>("Request not parsable", StatusCodes.Status400BadRequest);

            bool isProviderAvaliable = await ProviderIsValid(category.VendorId);
            if (!isProviderAvaliable)
                return new Response<Category>("Category couldnot be created due to provider is not exist", StatusCodes.Status424FailedDependency);

            if (category.Products.Count > 0)
            {
                foreach (Product subservice in category.Products)
                {
                    Product servizeProduct = await _context.Product.FindAsync(subservice.Id);
                    if (servizeProduct == null)
                    {
                        _context.Product.Add(subservice);
                    }
                }
            }

            if (category.Id > 0)
            {
                Category servizeCategory = await _context.Category.FindAsync(category.Id);
                if (servizeCategory != null)
                    return new Response<Category>("Given SerrvizeCategory Already Exist", StatusCodes.Status409Conflict);

                return new Response<Category>("failed Dependecy ", StatusCodes.Status424FailedDependency);

            }
            _context.Category.Add(category);
            return new Response<Category>(category, StatusCodes.Status200OK);
        }

        public async Task<Response<Category>> UpdateServiceCategory(Category category)
        {
            if (category == null)
                return new Response<Category>("Request not parsable", StatusCodes.Status400BadRequest);

            bool isProviderAvaliable = await ProviderIsValid(category.VendorId);
            if (!isProviderAvaliable)
                return new Response<Category>("Category couldnot be created due to provider is not exist", StatusCodes.Status424FailedDependency);

            if (category.Products.Count > 0)
            {
                foreach (Product subservice in category.Products)
                {
                    Product servizeProduct = await _context.Product.FindAsync(subservice.Id);
                    if (servizeProduct != null)
                    {
                        _context.Product.Update(subservice);
                    }
                }
            }

            if (category.Id > 0)
            {
                Category servizeCategory = await _context.Category.FindAsync(category.Id);
                if (servizeCategory != null)
                    return new Response<Category>("Given SerrvizeCategory Already Exist", StatusCodes.Status409Conflict);

                return new Response<Category>("failed Dependecy ", StatusCodes.Status424FailedDependency);

            }
            _context.Category.Update(category);
            return new Response<Category>(category, StatusCodes.Status200OK);
        }


        private async Task<bool> ProviderIsValid(int? VendorId)
        {
            bool isValid = false;
            if (VendorId != null)
            {
                Vendor provider = await _context.Vendor.AsNoTracking().SingleOrDefaultAsync(i => i.Id == VendorId);
                if (provider != null)
                    isValid = true;
            }
            return isValid;
        }
    }
}
