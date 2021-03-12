using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Servize.Domain.Model.Provider;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class ProviderRespository :BaseRepository<ServizeDBContext>
    {
        private readonly ServizeDBContext _context;
        public ProviderRespository(ServizeDBContext dBContext):base(dBContext)
        {
            _context = dBContext;
        }

        public async Task<Response<IList<Provider>>> GetAllServizeProviderList()
        {
            try
            {
                List<Provider> servizeProviderList = await _context.Provider.Include(i=>i.ServiceCategories)
                                                                                          .ThenInclude(i=>i.SubServices)
                                                                                          .AsNoTracking()
                                                                                        .ToListAsync();
                return new Response<IList<Provider>>(servizeProviderList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<IList<Provider>>($"Failed to get ServiceProviderList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }
        public async Task<Response<IList<Provider>>> GetAllServizeProviderByModeType(int modeType)
        {
            try
            {
                List<Provider> servizeProviderList = await _context.Provider.Where(e => Convert.ToInt32(e.ModeType) == modeType)
                                                                                          //  .Include(i => i.ServiceCategories)
                                                                                        //  .ThenInclude(i => i.SubServices)
                                                                                          .AsNoTracking()
                                                                                        .ToListAsync();
                if (servizeProviderList.Count() < 1)
                    return new Response<IList<Provider>>("Failed to get ServiceProviderList ", StatusCodes.Status404NotFound);


                return new Response<IList<Provider>>(servizeProviderList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<IList<Provider>>($"Failed to get ServiceProviderList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<Response<Provider>> GetAllServizeProviderById(string Id)
        {
            try
            {
                Provider servizeProvider = await _context.Provider
                    //.Include(i=>i.ServiceCategories)
                                                                                   // .ThenInclude(i=>i.SubServices)
                                                                                 .AsNoTracking()
                                                                                .SingleOrDefaultAsync(c => c.UserId == Id);
                if (servizeProvider == null)
                    return new Response<Provider>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<Provider>(servizeProvider, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<Provider>($"Failed to get ServiceProvide Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }
     
        public async Task<Response<Provider>> UpdateServizeProvider(Provider servizeProvider)
        {
            try
            {
                if (servizeProvider == null)
                    return new Response<Provider>("Request Not Parsable", StatusCodes.Status400BadRequest);
                Provider serviceProviderEntity = await _context.Provider.Include(i=>i.ServiceCategories)
                                                                                         .ThenInclude(i=>i.SubServices)          
                                                                                         .AsNoTracking()
                                                                                        .SingleOrDefaultAsync(c => c.Id == servizeProvider.Id);
                if (serviceProviderEntity == null)
                {
                    return new Response<Provider>("Provider not found", StatusCodes.Status404NotFound);
                }
                _context.Update(servizeProvider);
                await _context.SaveChangesAsync();
                return new Response<Provider>(servizeProvider, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                Log.Logger.Information(ex.Message);
                return new Response<Provider>($"Failed to Add ServiceProvide Error:{ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }

      
    }
}
