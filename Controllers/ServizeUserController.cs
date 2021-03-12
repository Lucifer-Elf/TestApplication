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
    public class ServizeUserController : ControllerBase
    {
        //UserClientDTO
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeUserServices _services;

        public ServizeUserController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        Utilities utitlity
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new ServizeUserServices(dbContext, mapper, utitlity);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("GetALL")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<UserClientDTO>>> GetAllServizeUserList()
        {

            Response<IList<UserClientDTO>> response = await _services.GetAllServizeUserList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<UserClientDTO>> GetServizeUserById(string id)
        {
            Response<UserClientDTO> response = await _services.GetAllServizeUserById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


    }
}
