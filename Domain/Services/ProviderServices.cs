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
using Microsoft.EntityFrameworkCore;
using Servize.Utility.Logging;

namespace Servize.Domain.Services
{
    public class ProviderServices
    {
        private readonly ProviderRespository _respository;
        private readonly ContextTransaction _transaction;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public ProviderServices(ServizeDBContext dbcontext,
            IMapper mapper, ContextTransaction transaction, Utilities utility)
        {
            _respository = new ProviderRespository(dbcontext);
            _transaction = transaction;
            _mapper = mapper;
            _utility = utility;

        }

        public async Task<Response<IList<ProviderDTO>>> GetAllServizeProviderList()
        {
            try
            {
                Response<IList<Provider>> response = await _respository.GetAllServizeProviderList();

                if (response.IsSuccessStatusCode())
                {
                    IList<ProviderDTO> serviceDTO = _mapper.Map<IList<Provider>, IList<ProviderDTO>>(response.Resource);
                    return new Response<IList<ProviderDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ProviderDTO>>("Failed to Load ServizeProvider List", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<ProviderDTO>>($"Failed to Load ServizeProvider List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<IList<ProviderDTO>>> GetAllServizeProviderByModeType(int modeType)
        {

            try
            {
                Response<IList<Provider>> response = await _respository.GetAllServizeProviderByModeType(modeType);

                if (response.IsSuccessStatusCode())
                {
                    IList<ProviderDTO> serviceDTO = _mapper.Map<IList<Provider>, IList<ProviderDTO>>(response.Resource);
                    return new Response<IList<ProviderDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ProviderDTO>>("Failed to Load ServizeProvider List", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<ProviderDTO>>($"Failed to Load ServizeProvider List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ProviderDTO>> GetAllServizeProviderById(string Id)
        {
            try
            {
                Response<Provider> response = await _respository.GetAllServizeProviderById(Id);
                if (response.IsSuccessStatusCode())
                {
                    ProviderDTO serviceDTO = _mapper.Map<Provider, ProviderDTO>(response.Resource);
                    return new Response<ProviderDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ProviderDTO>("Failed to Load ServizeProvider With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ProviderDTO>($"{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ProviderDTO>> PatchServizeProvider(ProviderDTO providerDTO)
        {
            Logger.LogInformation(0, "Patch for Product  service started !");
            try
            {
                if (providerDTO == null) return new Response<ProviderDTO>("Failed to Load ServizeProvider With Specific Id", StatusCodes.Status400BadRequest);

                Provider providerEntity = await _respository.GetContext().Provider.SingleOrDefaultAsync(p => p.Id == providerDTO.Id);
                if (providerEntity == null)
                    return new Response<ProviderDTO>("failed to find provider", StatusCodes.Status404NotFound);

                PatchEntities.PatchEntity<ProviderDTO, Provider>(_respository.GetContext(), _mapper, providerEntity, providerDTO);

                await _transaction.CompleteAsync();
                ProviderDTO mappedResponse = _mapper.Map<Provider, ProviderDTO>(providerEntity);
                return new Response<ProviderDTO>(mappedResponse, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new Response<ProviderDTO>("Provider Could Not be Updated", StatusCodes.Status200OK);
            }
            finally
            {
                Logger.LogInformation(0, "Patch for Product  service finished !");
            }

        }

        /*
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

        }*/
        public async Task<Response<ProviderDTO>> UpdateServizeProvider(ProviderDTO servizeProviderDTO)
        {
            Logger.LogInformation(0, "Update for Product  service started !");
            try
            {
                Provider serviceProvider = _mapper.Map<ProviderDTO, Provider>(servizeProviderDTO);

                Response<Provider> response = await _respository.UpdateServizeProvider(serviceProvider);
                if (response.IsSuccessStatusCode())
                {
                    ProviderDTO serviceDTO = _mapper.Map<Provider, ProviderDTO>(response.Resource);

                    return new Response<ProviderDTO>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<ProviderDTO>("Error while adding data in provider ", StatusCodes.Status500InternalServerError);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<ProviderDTO>($"Failed to Add ServizeProvider Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "Update for Product  service finished !");
            }
        }

       

    }
}
