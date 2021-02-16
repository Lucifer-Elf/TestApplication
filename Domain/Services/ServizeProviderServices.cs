using AutoMapper;
using Servize.Domain.Repositories;
using Servize.DTO.PROVIDER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Servize.Utility;
using Microsoft.AspNetCore.Http;
using Servize.Domain.Model;
using Servize.Domain.Model.Provider;
using Serilog;

namespace Servize.Domain.Services
{
    public class ServizeProviderServices
    {
        private readonly ServizeProviderRespository _respository;
        private readonly IMapper _mapper;
        public ServizeProviderServices(ServizeDBContext dbcontext, ServizeProviderRespository repository, IMapper mapper)
        {
            _respository = new ServizeProviderRespository(dbcontext);
            _mapper = mapper;

        }

        public async Task<Response<IList<ServizeProviderDTO>>> GetAllServizeProviderList()
        {
            try
            {
                Response<IList<ServizeProvider>> response = await _respository.GetAllServizeProviderList();

                if (response.IsSuccessStatusCode())
                {
                    IList<ServizeProviderDTO> serviceDTO = _mapper.Map<IList<ServizeProvider>, IList<ServizeProviderDTO>>(response.Resource);
                    return new Response<IList<ServizeProviderDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ServizeProviderDTO>>("Failed to Load ServizeProvider List", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<ServizeProviderDTO>>($"Failed to Load ServizeProvider List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<IList<ServizeProviderDTO>>> GetAllServizeProviderByModeType(int modeType)
        {

            try
            {
                Response<IList<ServizeProvider>> response = await _respository.GetAllServizeProviderByModeType(modeType);

                if (response.IsSuccessStatusCode())
                {
                    IList<ServizeProviderDTO> serviceDTO = _mapper.Map<IList<ServizeProvider>, IList<ServizeProviderDTO>>(response.Resource);
                    return new Response<IList<ServizeProviderDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ServizeProviderDTO>>("Failed to Load ServizeProvider List", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<ServizeProviderDTO>>($"Failed to Load ServizeProvider List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ServizeProviderDTO>> GetAllServizeProviderById(int Id)
        {
            try
            {
                Response<ServizeProvider> response = await _respository.GetAllServizeProviderById(Id);
                if (response.IsSuccessStatusCode())
                {
                    ServizeProviderDTO serviceDTO = _mapper.Map<ServizeProvider, ServizeProviderDTO>(response.Resource);
                    return new Response<ServizeProviderDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ServizeProviderDTO>("Failed to Load ServizeProvider With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ServizeProviderDTO>($"Failed to Load ServizeProvider Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }


        public async Task<Response<ServizeProviderDTO>> AddServizeProvider(ServizeProviderDTO servizeProviderDTO)
        {
            try
            {
                ServizeProvider serviceProvider = _mapper.Map<ServizeProviderDTO, ServizeProvider>(servizeProviderDTO);

                Response<ServizeProvider> response = await _respository.AddServizeProvider(serviceProvider);
                if (response.IsSuccessStatusCode())
                {
                    ServizeProviderDTO serviceDTO = _mapper.Map<ServizeProvider, ServizeProviderDTO>(response.Resource);
                    return new Response<ServizeProviderDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ServizeProviderDTO>("Failed to Add ServizeProvider With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ServizeProviderDTO>($"Failed to Add ServizeProvider Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<Response<IList<string>>> GetAllServizeProviderCategory()
        {
            try
            {
                return  await _respository.GetAllServizeProviderCategory();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while get List of Categories");
                return new Response<IList<string>>("Failed to load error", StatusCodes.Status500InternalServerError);
            
            }
        
        }

    }
}
