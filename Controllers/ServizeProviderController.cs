using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servize.Domain.Repositories;
using Servize.Domain.Services;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    [ApiController]
    [Route("[controller]")]   
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
        [Authorize]
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ServizeProviderDTO>>> GetAllServiceProviderList()
        {
            Response<IList<ServizeProviderDTO>> response = await _services.GetAllServizeProviderList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [Authorize]
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ServizeProviderDTO>> GetServiceProviderById(int id)
        {           
            Response<ServizeProviderDTO> response = await _services.GetAllServizeProviderById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [Authorize]
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

