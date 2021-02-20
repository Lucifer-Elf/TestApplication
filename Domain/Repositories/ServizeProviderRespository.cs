using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model.Provider;
using Servize.Domain.Model.Account;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Servize.Domain.Repositories
{
    public class ServizeProviderRespository
    {
        private readonly ServizeDBContext _context;
        public ServizeProviderRespository(ServizeDBContext dBContext)
        {
            _context = dBContext;
        }

        public async Task<Response<IList<ServizeProvider>>> GetAllServizeProviderList()
        {
            try
            {
                List<ServizeProvider> servizeProviderList = await _context.ServizeProvider
                                                                                        .Include(i=>i.ServiceCategories)
                                                                                          .ThenInclude(i=>i.SubServices)
                                                                                          .AsNoTracking()
                                                                                        .ToListAsync();
                return new Response<IList<ServizeProvider>>(servizeProviderList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<IList<ServizeProvider>>($"Failed to get ServiceProviderList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }
        public async Task<Response<IList<ServizeProvider>>> GetAllServizeProviderByModeType(int modeType)
        {
            try
            {
                List<ServizeProvider> servizeProviderList = await _context.ServizeProvider.Where(e => Convert.ToInt32(e.ModeType) == modeType)
                                                                                          //  .Include(i => i.ServiceCategories)
                                                                                        //  .ThenInclude(i => i.SubServices)
                                                                                          .AsNoTracking()
                                                                                        .ToListAsync();
                if (servizeProviderList.Count() < 1)
                    return new Response<IList<ServizeProvider>>("Failed to get ServiceProviderList ", StatusCodes.Status404NotFound);


                return new Response<IList<ServizeProvider>>(servizeProviderList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<IList<ServizeProvider>>($"Failed to get ServiceProviderList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<Response<ServizeProvider>> GetAllServizeProviderById(int Id)
        {
            try
            {
                ServizeProvider servizeProvider = await _context.ServizeProvider
                    //.Include(i=>i.ServiceCategories)
                                                                                   // .ThenInclude(i=>i.SubServices)
                                                                                 .AsNoTracking()
                                                                                .SingleOrDefaultAsync(c => c.Id == Id);
                if (servizeProvider == null)
                    return new Response<ServizeProvider>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<ServizeProvider>(servizeProvider, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<ServizeProvider>($"Failed to get ServiceProvide Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }
     
        public async Task<Response<ServizeProvider>> UpdateServizeProvider(ServizeProvider servizeProvider)
        {
            try
            {
                if (servizeProvider == null)
                    return new Response<ServizeProvider>("Request Not Parsable", StatusCodes.Status400BadRequest);
                ServizeProvider serviceProviderEntity = await _context.ServizeProvider.Include(i=>i.ServiceCategories)
                                                                                         .ThenInclude(i=>i.SubServices)          
                                                                                         .AsNoTracking()
                                                                                        .SingleOrDefaultAsync(c => c.Id == servizeProvider.Id);
                if (serviceProviderEntity == null)
                {
                    return new Response<ServizeProvider>("Provider not found", StatusCodes.Status404NotFound);
                }
                _context.Update(servizeProvider);
                await _context.SaveChangesAsync();
                return new Response<ServizeProvider>(servizeProvider, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                Log.Logger.Information(ex.Message);
                return new Response<ServizeProvider>($"Failed to Add ServiceProvide Error:{ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<IList<string>>> GetAllServizeProviderCategory()
        {
            try
            {
                var result = await _context.ServizeProvider.Include(e => e.ServiceCategories)
                                                       .ThenInclude(e => e.Type)
                                                       .Select(e => e.ServiceCategories)
                                                       .ToListAsync();
                if (result.Count() < 0)
                {
                    List<string> categoryList = new List<string>();
                    foreach (ServizeCategory category in result)
                    {
                        string type = category.Type.ToString();
                        if (!categoryList.Contains(type))
                            categoryList.Add(type);
                    }
                    return new Response<IList<string>>(categoryList, StatusCodes.Status404NotFound);
                }
                return new Response<IList<string>>("testttt", StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new Response<IList<string>>($"EXception :{ex.Message}", StatusCodes.Status200OK);

            }


        }
    }
}
