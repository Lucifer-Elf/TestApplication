using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Services;
using Servize.DTO.USER;
using Servize.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Controllers
{
   // [Authorize(Roles = UserRoles.Client + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
      
        private readonly ClientServices _services;

        public ClientController(ServizeDBContext dbContext,
                                         IMapper mapper, ContextTransaction transaction,
                                        Utilities utitlity )
        {        
            _services = new ClientServices(dbContext, mapper,transaction, utitlity);
        }

       // [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]     
        [Produces("application/json")]
        public async Task<ActionResult<IList<ClientDTO>>> GetAllUserList()
        {

            Response<IList<ClientDTO>> response = await _services.GetAllUserList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

       

        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ClientDTO>> GetUserById(string id)
        {
            Response<ClientDTO> response = await _services.GetUserById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPatch]
        public async Task<ActionResult<ClientDTO>> PatchClientDetails(ClientDTO clientDTO)
        {
            Response<ClientDTO> response = await _services.PatchClientDetails(clientDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


    }
}
