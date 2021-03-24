using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servize.Domain.Enums;
using Servize.Domain.Services;
using Servize.DTO;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Servize.Controllers
{
    [Authorize(Roles = UserRoles.Vendor + "," + UserRoles.Admin)]
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _services;

        public ProductController(ServizeDBContext dbContext,
                                         IMapper mapper, ContextTransaction transaction,
                                        Utilities utitlity)
        {
            _services = new ProductService(dbContext, mapper, transaction, utitlity);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IList<ProductDTO>>> GetAllProductList()
        {

            Response<IList<ProductDTO>> response = await _services.GetAllProductList();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }



        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            Response<ProductDTO> response = await _services.GetProductById(id);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPatch]
        public async Task<ActionResult<ProductDTO>> PatchDetails(ProductDTO ProductDTO)
        {
            Response<ProductDTO> response = await _services.PatchDetails(ProductDTO);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);

            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }
    }
}
