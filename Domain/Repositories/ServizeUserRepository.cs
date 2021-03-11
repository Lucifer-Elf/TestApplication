using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Servize.Domain.Model.Client;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class ServizeUserRepository
    {
        private readonly ServizeDBContext _context;
        public ServizeUserRepository(ServizeDBContext dBContext)
        {
            _context = dBContext;
        }

        public async Task<Response<IList<UserClient>>> GetAllServizeUserList()
        {
            try
            {
                List<UserClient> servizeProviderList = await _context.UserClient .AsNoTracking()
                                                                                        .ToListAsync();
                return new Response<IList<UserClient>>(servizeProviderList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<IList<UserClient>>($"Failed to get ServiceProviderList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }
       
        public async Task<Response<UserClient>> GetAllServizeUserById(string Id)
        {
            try
            {
                UserClient servizeProvider = await _context.UserClient.AsNoTracking()
                                                                                .SingleOrDefaultAsync(c => c.UserId == Id);
                if (servizeProvider == null)
                    return new Response<UserClient>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<UserClient>(servizeProvider, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                return new Response<UserClient>($"Failed to get ServiceProvide Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

    }
}
