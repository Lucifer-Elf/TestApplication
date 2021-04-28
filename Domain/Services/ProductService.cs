using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Servize.Domain.Model.VendorModel;
using Servize.Domain.Repositories;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using Servize.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Domain.Services
{
    public class ProductService
    {
        private ServizeDBContext _dbContext;
        private IMapper _mapper;
        private ContextTransaction _transaction;
        private Utilities _utitlity;
        private readonly ProductRepository _respository;

        public ProductService(ServizeDBContext dbContext, IMapper mapper, ContextTransaction transaction, Utilities utitlity)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
            this._transaction = transaction;
            this._utitlity = utitlity;
            _respository = new ProductRepository(dbContext);
        }

        public async Task<Response<IList<ProductDTO>>> GetAllProductList()
        {
            Logger.LogInformation(0, "Get All Product service started !");
            try
            {
                Response<IList<Product>> response = await _respository.GetAllProductList();

                if (response.IsSuccessStatusCode())
                {
                    IList<ProductDTO> serviceDTO = _mapper.Map<IList<Product>, IList<ProductDTO>>(response.Resource);
                    return new Response<IList<ProductDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<ProductDTO>>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<IList<ProductDTO>>($"Failed to Load Product List Error", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "Get All Product service finished !");
            }
        }



        public async Task<Response<ProductDTO>> GetProductById(int Id)
        {
            Logger.LogInformation(0, "Get All Product By id service started !");
            try
            {
                Response<Product> response = await _respository.GetProductById(Id);
                if (response.IsSuccessStatusCode())
                {
                    ProductDTO serviceDTO = _mapper.Map<Product, ProductDTO>(response.Resource);
                    return new Response<ProductDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<ProductDTO>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<ProductDTO>($"Failed to Get Product Id", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "Get All Product By id  service finished !");
            }
        }

        public async Task<Response<ProductDTO>> PatchDetails(ProductDTO productDTO)
        {
            Logger.LogInformation(0, "Patch for Product  service started !");
            try
            {
                if (productDTO == null) return new Response<ProductDTO>("Failed to Load Product With Specific Id", StatusCodes.Status400BadRequest);

                Product ProductEntity = await _respository.GetContext().Product.SingleOrDefaultAsync(p => p.Id == productDTO.Id);
                if (ProductEntity == null)
                    return new Response<ProductDTO>("failed to find Product", StatusCodes.Status404NotFound);

                PatchEntities.PatchEntity<ProductDTO, Product>(_respository.GetContext(), _mapper, ProductEntity, productDTO);

                await _transaction.CompleteAsync();
                ProductDTO mappedResponse = _mapper.Map<Product, ProductDTO>(ProductEntity);
                return new Response<ProductDTO>(mappedResponse, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new Response<ProductDTO>("Product Could Not be Updated", StatusCodes.Status200OK);
            }
            finally
            {
                Logger.LogInformation(0, "Patch for Product  service finished !");
            }
        }

    }
}
