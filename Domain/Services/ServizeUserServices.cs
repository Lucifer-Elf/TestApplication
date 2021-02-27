using AutoMapper;
using Microsoft.AspNetCore.Http;
using Servize.Domain.Model.Client;
using Servize.Domain.Repositories;
using Servize.DTO.USER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class ServizeUserServices
    {
        private readonly ServizeUserRepository _respository;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public ServizeUserServices(ServizeDBContext dbcontext,
            IMapper mapper, Utilities utility)
        {
            _respository = new ServizeUserRepository(dbcontext);
            _mapper = mapper;
            _utility = utility;

        }

        public async Task<Response<IList<UserClientDTO>>> GetAllServizeUserList()
        {
            try
            {
                Response<IList<UserClient>> response = await _respository.GetAllServizeUserList();

                if (response.IsSuccessStatusCode())
                {
                    IList<UserClientDTO> serviceDTO = _mapper.Map<IList<UserClient>, IList<UserClientDTO>>(response.Resource);
                    return new Response<IList<UserClientDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<UserClientDTO>>("Failed to Load User List", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<UserClientDTO>>($"Failed to Load user List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

      

        public async Task<Response<UserClientDTO>> GetAllServizeUserById(string Id)
        {
            try
            {
                Response<UserClient> response = await _respository.GetAllServizeUserById(Id);
                if (response.IsSuccessStatusCode())
                {
                    UserClientDTO serviceDTO = _mapper.Map<UserClient, UserClientDTO>(response.Resource);
                    return new Response<UserClientDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<UserClientDTO>("Failed to Load User With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<UserClientDTO>($"Failed to Load User Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

    }
}
