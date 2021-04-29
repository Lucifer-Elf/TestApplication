using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Servize.Domain.Model;
using Servize.Domain.Repositories;
using Servize.DTO.USER;
using Servize.Utility;
using Servize.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class ClientServices
    {
        private readonly ClientRepository _repository;
        private readonly ContextTransaction _transaction;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public ClientServices(ServizeDBContext dbcontext, IMapper mapper, ContextTransaction transaction, Utilities utility)
        {
            _repository = new ClientRepository(dbcontext);
            _transaction = transaction;
            _mapper = mapper;
            _utility = utility;

        }

        public async Task<Response<IList<ClientDTO>>> GetAllUserList()
        {
            Logger.LogInformation(0, "GetAll Client service Started !");
            try
            {
                Response<IList<Client>> response = await _repository.GetAllClientList();

                if (response.IsSuccessStatusCode())
                {
                    IList<ClientDTO> serviceDTO = _mapper.Map<IList<Client>, IList<ClientDTO>>(response.Resource);
                    return new Response<IList<ClientDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ClientDTO>>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<IList<ClientDTO>>($"Failed to Load user List Error", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "GetAll Client service finished !");
            }
        }



        public async Task<Response<ClientDTO>> GetUserById(string id)
        {
            Logger.LogInformation(0, "Get Client By id service started !");
            try
            {
                Response<Client> response = await _repository.GetAllServizeUserById(id);
                if (response.IsSuccessStatusCode())
                {
                    ClientDTO serviceDTO = _mapper.Map<Client, ClientDTO>(response.Resource);
                    return new Response<ClientDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ClientDTO>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<ClientDTO>($"Failed to Get User Error By Id", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "Get Client By id service finished !");
            }
        }

        public async Task<Response<ClientDTO>> PatchClientDetails(ClientDTO clientDTO)
        {
            Logger.LogInformation(0, "Patch Service for Client service started !");
            try
            {
                if (clientDTO == null) return new Response<ClientDTO>("Failed to Load Client With Specific Id", StatusCodes.Status400BadRequest);

                Client clientEntity = await _repository.GetContext().Client.SingleOrDefaultAsync(p => p.Id == clientDTO.Id);
                if (clientEntity == null)
                    return new Response<ClientDTO>("failed to find Client", StatusCodes.Status404NotFound);

                PatchEntities.PatchEntity<ClientDTO, Client>(_repository.GetContext(), _mapper, clientEntity, clientDTO);

                await _transaction.CompleteAsync();
                ClientDTO mappedResponse = _mapper.Map<Client, ClientDTO>(clientEntity);
                return new Response<ClientDTO>(mappedResponse, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new Response<ClientDTO>("Client Could Not be Updated", StatusCodes.Status200OK);
            }

            finally
            {
                Logger.LogInformation(0, "Patch Service for Client service finished !");
            }
        }
    }
}
