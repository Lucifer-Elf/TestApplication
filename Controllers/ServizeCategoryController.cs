using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Servize.Authentication;
using Servize.Domain.Services;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    //[Authorize(Roles = UserRoles.Provider + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class ServizeCategoryController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeCategoryService _services;


        public ServizeCategoryController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        Utilities utitlity
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new ServizeCategoryService(dbContext, mapper, utitlity);
        }


        [HttpGet]
        [Route("Get")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ServizeCategoryDTO>>> GetAllCategoryList()
        {
            Response<IList<ServizeCategoryDTO>> response = await _services.GetAllCategoryList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ServizeCategoryDTO>> GetAllCategoryById(int id)
        {
            Response<ServizeCategoryDTO> response = await _services.GetAllCategoryById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        public async Task<ActionResult<ServizeCategoryDTO>> AddServiceCategory([FromBody] ServizeCategoryDTO servizeCategoryDTO)
        {

            Response<ServizeCategoryDTO> response    = await _services.AddServiceCategory(servizeCategoryDTO);
                if (response.IsSuccessStatusCode())
                    return Ok(response.Resource);
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }
    }
}
