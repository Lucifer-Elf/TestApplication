using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servize.Domain.Enums;
using Servize.Domain.Repositories;
using Servize.Domain.Services;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Servize.Domain.Enums.ServizeEnum;

namespace Servize.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [ApiController]
    [Route("api/[controller]")]   
    public class ServizeProviderController : ControllerBase
    {
        private readonly ServizeProviderServices _services;
        public ServizeProviderController(ServizeDBContext dbContext,
                                         ServizeProviderRespository repository,
                                         IMapper mapper
                                         )
        {

            _services = new ServizeProviderServices(dbContext, repository, mapper);

        }
       
        [HttpGet]
        [Route("Get")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ServizeProviderDTO>>> GetAllServiceProviderList()
        {
            Response<IList<ServizeProviderDTO>> response = await _services.GetAllServizeProviderList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

      
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ServizeProviderDTO>> GetServiceProviderById(int id)
        {           
            Response<ServizeProviderDTO> response = await _services.GetAllServizeProviderById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpGet("modetype/{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ServizeProviderDTO>>> GetAllServiceProviderByModeType(int modeType)
        {

            Response<IList<ServizeProviderDTO>> response = await _services.GetAllServizeProviderList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpPost]
        [Route("AddProvider")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<ServizeProviderDTO>> AddServiceProvider([FromBody] ServizeProviderDTO servizeProviderDTO)
        {
            Response<ServizeProviderDTO> response = await _services.AddServizeProvider(servizeProviderDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }
    }
} 

