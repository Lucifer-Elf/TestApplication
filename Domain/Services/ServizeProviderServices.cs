﻿using AutoMapper;
using Servize.Domain.Repositories;
using Servize.DTO.PROVIDER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Servize.Utility;
using Microsoft.AspNetCore.Http;
using Servize.Domain.Model;

namespace Servize.Domain.Services
{
    public class ServizeProviderServices
    {
        private readonly ServizeProviderRespository _respository;
        private readonly IMapper _mapper;
        public ServizeProviderServices(ServizeDBContext dbcontext, ServizeProviderRespository repository, IMapper mapper)
        {
            _respository = new ServizeProviderRespository(dbcontext);
        }

        public async Task<Response<IList<ServizeProviderDTO>>> GetAllServizeProviderList()
        {
            try
            {
                Response<IList<ServizeProvider>> response = await _respository.GetAllServizeProviderList();

                if (response.IsSuccessStatusCode())
                {
                    IList<ServizeProviderDTO> serviceDTO = _mapper.Map<IList<ServizeProvider>, IList<ServizeProviderDTO>>(response.Resource);
                    new Response<IList<ServizeProviderDTO>>(serviceDTO, StatusCodes.Status200OK);
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
                    new Response<ServizeProviderDTO>(serviceDTO, StatusCodes.Status200OK);
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
                    new Response<ServizeProviderDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ServizeProviderDTO>("Failed to Add ServizeProvider With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ServizeProviderDTO>($"Failed to Add ServizeProvider Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }

    }
}