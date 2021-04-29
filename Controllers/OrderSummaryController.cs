using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Servize.Domain.Services;
using Servize.DTO;
using Servize.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderSummaryController : ControllerBase
    {
        private readonly OrderSummaryServices _services;
        public OrderSummaryController(ServizeDBContext dbContext, IMapper mapper, ContextTransaction transaction, Utilities utilities)
        {
            _services = new OrderSummaryServices(dbContext, mapper, transaction, utilities);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IList<OrderSummaryDTO>>> GetAllOrderSummary()
        {
            Response<IList<OrderSummaryDTO>> response = await _services.GetAllOrderSummary();
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }
    }
}
