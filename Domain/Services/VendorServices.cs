using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class VendorServices
    {
        private readonly VendorRespository _respository;
        private readonly ContextTransaction _transaction;
        private readonly IMapper _mapper;
        private readonly Utilities _utility;

        public VendorServices(ServizeDBContext dbcontext,
            IMapper mapper, ContextTransaction transaction, Utilities utility)
        {
            _respository = new VendorRespository(dbcontext);
            _transaction = transaction;
            _mapper = mapper;
            _utility = utility;

        }

        public async Task<Response<IList<VendorDTO>>> GetAllVendorList()
        {
            try
            {
                Response<IList<Vendor>> response = await _respository.GetAllVendorList();

                if (response.IsSuccessStatusCode())
                {
                    IList<VendorDTO> serviceDTO = _mapper.Map<IList<Vendor>, IList<VendorDTO>>(response.Resource);
                    return new Response<IList<VendorDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<VendorDTO>>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<VendorDTO>>($"Failed to Load Vendor List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<IList<VendorDTO>>> GetAllVendorByModeType(int modeType)
        {

            try
            {
                Response<IList<Vendor>> response = await _respository.GetAllVendorByModeType(modeType);

                if (response.IsSuccessStatusCode())
                {
                    IList<VendorDTO> serviceDTO = _mapper.Map<IList<Vendor>, IList<VendorDTO>>(response.Resource);
                    return new Response<IList<VendorDTO>>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<IList<VendorDTO>>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<IList<VendorDTO>>($"Failed to Load Vendor List Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<VendorDTO>> GetAllVendorById(string Id)
        {
            try
            {
                Response<Vendor> response = await _respository.GetAllVendorById(Id);
                if (response.IsSuccessStatusCode())
                {
                    VendorDTO venderDTO = _mapper.Map<Vendor, VendorDTO>(response.Resource);
                    return new Response<VendorDTO>(venderDTO, StatusCodes.Status200OK);
                }
                return new Response<VendorDTO>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<VendorDTO>($"Failed to Load Vendor With Specific Id", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<Response<VendorDTO>> PatchVendor(VendorDTO vendorDTO)
        {
            Logger.LogInformation(0, "Patch for Product  service started !");
            try
            {
                if (vendorDTO == null) return new Response<VendorDTO>("Failed to Load Vendor With Specific Id", StatusCodes.Status400BadRequest);

                Vendor providerEntity = await _respository.GetContext().Vendor.SingleOrDefaultAsync(p => p.Id == vendorDTO.Id);
                if (providerEntity == null)
                    return new Response<VendorDTO>("failed to find provider", StatusCodes.Status404NotFound);

                PatchEntities.PatchEntity<VendorDTO, Vendor>(_respository.GetContext(), _mapper, providerEntity, vendorDTO);

                await _transaction.CompleteAsync();
                VendorDTO mappedResponse = _mapper.Map<Vendor, VendorDTO>(providerEntity);
                return new Response<VendorDTO>(mappedResponse, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new Response<VendorDTO>("Provider Could Not be Updated", StatusCodes.Status200OK);
            }
            finally
            {
                Logger.LogInformation(0, "Patch for Product  service finished !");
            }

        }

        /*
        public async Task<Response<VendorDTO>> AddVendor(VendorDTO servizeProviderDTO)
        {
            try
            {
                Vendor serviceProvider = _mapper.Map<VendorDTO, Vendor>(servizeProviderDTO);

                Response<Vendor> response = await _respository.AddVendor(serviceProvider);
                if (response.IsSuccessStatusCode())
                {
                    VendorDTO serviceDTO = _mapper.Map<Vendor, VendorDTO>(response.Resource);
                    return new Response<VendorDTO>(serviceDTO, StatusCodes.Status200OK);
                }
                return new Response<VendorDTO>("Failed to Add Vendor With Specific Id", response.StatusCode);
            }
            catch (Exception e)
            {
                return new Response<VendorDTO>($"Failed to Add Vendor Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }*/
        public async Task<Response<VendorDTO>> UpdateVendor(VendorDTO servizeProviderDTO)
        {
            Logger.LogInformation(0, "Update for Product  service started !");
            try
            {
                Vendor vendor = _mapper.Map<VendorDTO, Vendor>(servizeProviderDTO);

                Response<Vendor> response = await _respository.UpdateVendor(vendor);
                if (response.IsSuccessStatusCode())
                {
                    VendorDTO serviceDTO = _mapper.Map<Vendor, VendorDTO>(response.Resource);

                    return new Response<VendorDTO>(serviceDTO, StatusCodes.Status200OK);
                }

                return new Response<VendorDTO>(response.Message, StatusCodes.Status500InternalServerError);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<VendorDTO>($"Failed to Add Vendor Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "Update for Product  service finished !");
            }
        }

       

    }
}
