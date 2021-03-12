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
        private readonly ProviderServices _services;

        public ServizeProviderController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager, ContextTransaction transaction,
                                        Utilities utitlity
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new ProviderServices(dbContext, mapper, transaction, utitlity);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("GetALL")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ProviderDTO>>> GetAllServizeProviderList()
        {

            Response<IList<ProviderDTO>> response = await _services.GetAllServizeProviderList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ProviderDTO>> GetServizeProviderById(string id)
        {
            Response<ProviderDTO> response = await _services.GetAllServizeProviderById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("modetype/{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ProviderDTO>>> GetAllServizeProviderByModeType(int modeType)
        {
            Response<IList<ProviderDTO>> response = await _services.GetAllServizeProviderByModeType(modeType);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPut]
        [Route("UpdateProvider")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<ProviderDTO>> UpdateServizeProvider(ProviderDTO servizeProviderDTO)
        {

            Response<ProviderDTO> response = await _services.UpdateServizeProvider(servizeProviderDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);

        }

        public async Task<ActionResult<ProviderDTO>> PatchServizeProvider(ProviderDTO providerDTO)
        {
            Response<ProviderDTO> response = await _services.PatchServizeProvider(providerDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }
    }
}

