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
    [Authorize(Roles = UserRoles.Client + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        //UserClientDTO
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ClientServices _services;

        public ClientController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        ContextTransaction transaction,
                                        Utilities utitlity
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new ClientServices(dbContext, mapper,transaction, utitlity);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("GetALL")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ClientDTO>>> GetAllServizeUserList()
        {

            Response<IList<ClientDTO>> response = await _services.GetAllServizeUserList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ClientDTO>> GetServizeUserById(string id)
        {
            Response<ClientDTO> response = await _services.GetAllServizeUserById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        public async Task<ActionResult<ClientDTO>> PatchClientDetails(ClientDTO clientDTO)
        {
            Response<ClientDTO> response = await _services.PatchClientDetails(clientDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


    }
}
