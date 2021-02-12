using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                List<ServizeProvider> servizeProviderList = await _context.ServizeProvider.ToListAsync();
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
                ServizeProvider servizeProvider = await _context.ServizeProvider.SingleOrDefaultAsync(c => c.Id == Id);
                if (servizeProvider == null)
                    return new Response<ServizeProvider>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<ServizeProvider>(servizeProvider, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<ServizeProvider>($"Failed to get ServiceProvide Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ServizeProvider>> AddServizeProvider(ServizeProvider servizeProvider)
        {
            try
            {
                if (servizeProvider == null)
                    return new Response<ServizeProvider>("Request Not Parsable", StatusCodes.Status400BadRequest);

                ServizeProvider serviceProviderEntity = await _context.ServizeProvider.SingleOrDefaultAsync(c => c.Id == servizeProvider.Id);
                if (serviceProviderEntity != null)
                    return new Response<ServizeProvider>("ServiceProvider with given Id Already exist", StatusCodes.Status409Conflict);
                servizeProvider.Id = 0;

                _context.ServizeProvider.Add(servizeProvider);

                return new Response<ServizeProvider>(servizeProvider, StatusCodes.Status200OK);
                

            }
            catch (Exception e)
            {
                return new Response<ServizeProvider>($"Failed to Add ServiceProvide Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }        
        }
    }
}
