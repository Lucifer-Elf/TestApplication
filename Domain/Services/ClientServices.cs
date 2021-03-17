﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Servize.Domain.Model;
using Servize.Domain.Repositories;
using Servize.DTO.USER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class ClientServices
    {
        private readonly ClientRepository _respository;
        private readonly ContextTransaction _transaction;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public ClientServices(ServizeDBContext dbcontext,
            IMapper mapper, ContextTransaction transaction, Utilities utility)
        {
            _respository = new ClientRepository(dbcontext);
            _transaction = transaction;
            _mapper = mapper;
            _utility = utility;

        }

        public async Task<Response<IList<ClientDTO>>> GetAllUserList()
        {
            try
            {
                Response<IList<Client>> response = await _respository.GetAllServizeUserList();

                if (response.IsSuccessStatusCode())
                {
                    IList<ClientDTO> serviceDTO = _mapper.Map<IList<Client>, IList<ClientDTO>>(response.Resource);
                    return new Response<IList<ClientDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ClientDTO>>("Failed to Load User List", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<ClientDTO>>($"Failed to Load user List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

      

        public async Task<Response<ClientDTO>> GetUserById(string Id)
        {
            try
            {
                Response<Client> response = await _respository.GetAllServizeUserById(Id);
                if (response.IsSuccessStatusCode())
                {
                    ClientDTO serviceDTO = _mapper.Map<Client, ClientDTO>(response.Resource);
                    return new Response<ClientDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ClientDTO>("Failed to Load User With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ClientDTO>($"Failed to Load User Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ClientDTO>> PatchClientDetails(ClientDTO clientDTO)
        {
            try
            {
                if (clientDTO == null) return new Response<ClientDTO>("Failed to Load Client With Specific Id", StatusCodes.Status400BadRequest);

                Client clientEntity = await _respository.GetContext().Client.SingleOrDefaultAsync(p => p.Id == clientDTO.Id);
                if (clientEntity == null)
                    return new Response<ClientDTO>("failed to find Client", StatusCodes.Status404NotFound);

                PatchEntities.PatchEntity<ClientDTO, Client>(_respository.GetContext(), _mapper, clientEntity, clientDTO);

                await _transaction.CompleteAsync();
                ClientDTO mappedResponse = _mapper.Map<Client, ClientDTO>(clientEntity);
                return new Response<ClientDTO>(mappedResponse, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new Response<ClientDTO>("Client Could Not be Updated", StatusCodes.Status200OK);
            }

        }
    }
}