using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model.Provider;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class ServizeCategoryRepository
    {
        private readonly ServizeDBContext _context;
        public ServizeCategoryRepository(ServizeDBContext dBContext)
        {
            _context = dBContext;
        }

        public async Task<Response<IList<ServizeCategory>>> GetAllServizeCategoryList()
        {
            try
            {
                List<ServizeCategory> servizeProviderList = await _context.ServizeCategory.Include(i => i.SubServices).ToListAsync();
                return new Response<IList<ServizeCategory>>(servizeProviderList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<IList<ServizeCategory>>($"Failed to get ServiceCategoryList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<Response<ServizeCategory>> GetAllServizeCategoryById(int Id)
        {
            try
            {
                ServizeCategory servizeProvider = await _context.ServizeCategory.Include(i => i.SubServices)
                                                                                 .AsNoTracking()
                                                                                .SingleOrDefaultAsync(c => c.Id == Id);
                if (servizeProvider == null)
                    return new Response<ServizeCategory>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<ServizeCategory>(servizeProvider, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<ServizeCategory>($"Failed to get ServiceCategory Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ServizeCategory>> AddServiceCategory(ServizeCategory category)
        {
            if (category == null)
                return new Response<ServizeCategory>("Request not parsable", StatusCodes.Status400BadRequest);

            bool isProviderAvaliable = await ProviderIsValid(category.ProviderId);
            if (!isProviderAvaliable)
                return new Response<ServizeCategory>("Category couldnot be created due to provider is not exist", StatusCodes.Status424FailedDependency);

            if (category.Id > 0)
            {
                ServizeCategory servizeCategory = await _context.ServizeCategory.FindAsync(category.Id);
                if (servizeCategory != null)
                    return new Response<ServizeCategory>("Given SerrvizeCategory Already Exist", StatusCodes.Status409Conflict);

                return new Response<ServizeCategory>("failed Dependecy ", StatusCodes.Status424FailedDependency);

            }
            _context.ServizeCategory.Add(category);
            return new Response<ServizeCategory>(category, StatusCodes.Status200OK);
        }


        private async Task<bool> ProviderIsValid(int? ProviderId)
        {
            bool isValid = false;
            if (ProviderId != null)
            {
                ServizeProvider provider = await _context.ServizeProvider.AsNoTracking().SingleOrDefaultAsync(i => i.Id == ProviderId);
                if (provider != null)
                    isValid = true;
            }
            return isValid;
        }
    }
}
