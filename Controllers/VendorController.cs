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
    //[Authorize(Roles = UserRoles.Vendor + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly VendorServices _services;

        public VendorController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager, ContextTransaction transaction,
                                        Utilities utitlity
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new VendorServices(dbContext, mapper, transaction, utitlity);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]  
        [Produces("application/json")]
        public async Task<ActionResult<IList<VendorDTO>>> GetAllVendorList()
        {

            Response<IList<VendorDTO>> response = await _services.GetAllVendorList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<VendorDTO>> GetAllVendorById(string id)
        {
            Response<VendorDTO> response = await _services.GetAllVendorById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

       // [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("modetype/{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<VendorDTO>>> GetAllVendorByModeType(int modeType)
        {
            Response<IList<VendorDTO>> response = await _services.GetAllVendorByModeType(modeType);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPut]   
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<VendorDTO>> UpdateVendor(VendorDTO servizeProviderDTO)
        {

            Response<VendorDTO> response = await _services.UpdateVendor(servizeProviderDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);

        }

        [HttpPatch]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<VendorDTO>> PatchVendor(VendorDTO providerDTO)
        {
            Response<VendorDTO> response = await _services.PatchVendor(providerDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }
    }
}

