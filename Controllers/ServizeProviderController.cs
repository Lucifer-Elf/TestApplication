using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Services;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    [Authorize(Roles = UserRoles.Provider + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class ServizeProviderController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeProviderServices _services;

        public ServizeProviderController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        Utilities utitlity
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new ServizeProviderServices(dbContext, mapper, utitlity);
        }

        [Authorize(Roles =  UserRoles.Admin)]
        [HttpGet]
        [Route("GetALL")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ServizeProviderDTO>>> GetAllServizeProviderList()
        {

            Response<IList<ServizeProviderDTO>> response = await _services.GetAllServizeProviderList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ServizeProviderDTO>> GetServizeProviderById(string id)
        {
            Response<ServizeProviderDTO> response = await _services.GetAllServizeProviderById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpGet("modetype/{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ServizeProviderDTO>>> GetAllServizeProviderByModeType(int modeType)
        {
            Response<IList<ServizeProviderDTO>> response = await _services.GetAllServizeProviderByModeType(modeType);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPut]
        [Route("UpdateProvider")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<ServizeProviderDTO>> UpdateServizeProvider(ServizeProviderDTO servizeProviderDTO)
        {

            Response<ServizeProviderDTO> response = await _services.UpdateServizeProvider(servizeProviderDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);

        }

        /*
        //ServizeProvider/AddProvider
        [HttpPost]
        [Route("AddProvider")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<ServizeProviderDTO>> AddServiceProvider([FromBody] ServizeProviderDTO servizeProviderDTO,[FromHeader] string userId)
        {
           
            Response<ServizeProviderDTO> response = new Response<ServizeProviderDTO>("Failed to Add ServizeProvider With Specific Id", StatusCodes.Status500InternalServerError) ;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                servizeProviderDTO.UserId = userId;
                response = await _services.AddServizeProvider(servizeProviderDTO);
                if (response.IsSuccessStatusCode())
                    return Ok(response.Resource);
            }

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }
        */

        public async Task<ActionResult<IList<string>>> GetAllServizeProviderCategory()
        {

            Response<IList<string>> response = await _services.GetAllServizeProviderCategory();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);

        }
    }
}

