using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Authorize(Roles = UserRoles.Vendor + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CategoryService _services;


        public CategoryController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        ContextTransaction transaction
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new CategoryService(dbContext, mapper,transaction);
        }


        [HttpGet]
        [Route("Get")]
        [Produces("application/json")]
        public async Task<ActionResult<IList<CategoryDTO>>> GetAllCategoryList()
        {
            Response<IList<CategoryDTO>> response = await _services.GetAllCategoryList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<CategoryDTO>> GetAllCategoryById(int id)
        {
            Response<CategoryDTO> response = await _services.GetAllCategoryById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPost]
        [Route("AddCategory")]
        public async Task<ActionResult<CategoryDTO>> AddServiceCategory([FromBody] CategoryDTO categoryDTO)
        {
            Response<CategoryDTO> response    = await _services.PostCategory(categoryDTO);                
            if (response.IsSuccessStatusCode())
                    return Ok(response.Resource);
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPut]
        [Route("UpdateCategory")]
        public async Task<ActionResult<CategoryDTO>> UpdateServiceCategory([FromBody] CategoryDTO categoryDTO)
        {

            Response<CategoryDTO> response = await _services.UpdateServiceCategory(categoryDTO);

            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        
    }
}
