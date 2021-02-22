using AutoMapper;
using Microsoft.AspNetCore.Http;
using Servize.Domain.Model.Provider;
using Servize.Domain.Repositories;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class ServizeCategoryService
    {
        private readonly ServizeCategoryRepository _respository;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public ServizeCategoryService(ServizeDBContext dbcontext,
            IMapper mapper, Utilities utility)
        {
            _respository = new ServizeCategoryRepository(dbcontext);
            _mapper = mapper;
            _utility = utility;

        }
        public async Task<Response<IList<ServizeCategoryDTO>>> GetAllCategoryList()
        {
            try
            {
                Response<IList<ServizeCategory>> response = await _respository.GetAllServizeCategoryList();

                if (response.IsSuccessStatusCode())
                {
                    IList<ServizeCategoryDTO> serviceDTO = _mapper.Map<IList<ServizeCategory>, IList<ServizeCategoryDTO>>(response.Resource);
                    return new Response<IList<ServizeCategoryDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ServizeCategoryDTO>>("Failed to Load ServizeCategory List", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<ServizeCategoryDTO>>($"Failed to Load ServizeCategory List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }


        public async Task<Response<ServizeCategoryDTO>> GetAllCategoryById(int Id)
        {
            try
            {
                Response<ServizeCategory> response = await _respository.GetAllServizeCategoryById(Id);
                if (response.IsSuccessStatusCode())
                {
                    ServizeCategoryDTO serviceDTO = _mapper.Map<ServizeCategory, ServizeCategoryDTO>(response.Resource);
                    return new Response<ServizeCategoryDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ServizeCategoryDTO>("Failed to Load ServizeCategory With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ServizeCategoryDTO>($"Failed to Load ServizeCategory Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ServizeCategoryDTO>> AddServiceCategory(ServizeCategoryDTO servizeCategoryDTO )
        {
            try
            {
                ServizeCategory serviceCategory = _mapper.Map<ServizeCategoryDTO, ServizeCategory>(servizeCategoryDTO);

                Response<ServizeCategory> response = await _respository.AddServiceCategory(serviceCategory);
                if (response.IsSuccessStatusCode())
                {
                    await _utility.CompleteTransactionAsync();
                    ServizeCategoryDTO serviceDTO = _mapper.Map<ServizeCategory, ServizeCategoryDTO>(response.Resource);
                    return new Response<ServizeCategoryDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ServizeCategoryDTO>("Failed to Add ServizeCategory With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ServizeCategoryDTO>($"Failed to Add ServizeCategory Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<ServizeCategoryDTO>> UpdateServiceCategory(ServizeCategoryDTO servizeCategoryDTO)
        {
            try
            {
                ServizeCategory serviceCategory = _mapper.Map<ServizeCategoryDTO, ServizeCategory>(servizeCategoryDTO);

                Response<ServizeCategory> response = await _respository.UpdateServiceCategory(serviceCategory);
                if (response.IsSuccessStatusCode())
                {
                    await _utility.CompleteTransactionAsync();
                    ServizeCategoryDTO serviceDTO = _mapper.Map<ServizeCategory, ServizeCategoryDTO>(response.Resource);
                    return new Response<ServizeCategoryDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ServizeCategoryDTO>("Failed to Add ServizeCategory With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<ServizeCategoryDTO>($"Failed to Add ServizeCategory Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

    }
}
