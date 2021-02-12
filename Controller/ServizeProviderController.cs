using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Servize.Domain.Repositories;
using Servize.Domain.Services;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Servize.Controller
{
    [Route("[controller]")]
    [ApiController]
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



        [HttpPost]
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

