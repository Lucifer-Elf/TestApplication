using AutoMapper;
using Microsoft.AspNetCore.Http;
using Servize.Domain.Model.OrderDetail;
using Servize.Domain.Repositories;
using Servize.DTO;
using Servize.Utility;
using Servize.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class OrderSummaryServices
    {
        private readonly ContextTransaction _transaction;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;
        private readonly OrderSummaryRepository _repository;

        public OrderSummaryServices(ServizeDBContext dbContext, IMapper mapper, ContextTransaction transaction, Utilities utility)
        {
            _repository = new OrderSummaryRepository(dbContext);
            _mapper = mapper;
            _transaction = transaction;
            _utility = utility;

        }

        public async Task<Response<IList<OrderSummaryDTO>>> GetAllOrderSummary()
        {
            Logger.LogInformation(0, "GetAll OrderSummary service Started !");
            try
            {
                Response<IList<OrderSummary>> response = await _repository.GetAllOrderSummary();

                if (response.IsSuccessStatusCode())
                {
                    IList<OrderSummaryDTO> serviceDTO = _mapper.Map<IList<OrderSummary>, IList<OrderSummaryDTO>>(response.Resource);
                    return new Response<IList<OrderSummaryDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<OrderSummaryDTO>>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<IList<OrderSummaryDTO>>($"Failed to Load OrderSummary List Error", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "GetAll OrderSummary service finished !");
            }
        }
    }
}
