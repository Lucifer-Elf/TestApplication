using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Servize.Domain.Model.VendorModel;
using Servize.Utility;
using Servize.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class VendorRespository :BaseRepository<ServizeDBContext>
    {
        private readonly ServizeDBContext _context;
        public VendorRespository(ServizeDBContext dBContext):base(dBContext)
        {
            _context = dBContext;
        }

        public async Task<Response<IList<Vendor>>> GetAllVendorList()
        {
            try
            {
                List<Vendor> vendorList = await _context.Vendor.Include(i=>i.Categories)
                                                                                          .ThenInclude(i=>i.Products)
                                                                                          .AsNoTracking()
                                                                                        .ToListAsync();
                return new Response<IList<Vendor>>(vendorList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<IList<Vendor>>($"Failed to get VendorList Error:{e.Message}", StatusCodes.Status500InternalServerError);
            }

        }
        public async Task<Response<IList<Vendor>>> GetAllVendorByModeType(int modeType)
        {
            try
            {
                List<Vendor> vendorList = await _context.Vendor.Where(e => Convert.ToInt32(e.ModeType) == modeType)
                                                                                          .AsNoTracking()
                                                                                        .ToListAsync();
                if (vendorList.Count() < 1)
                    return new Response<IList<Vendor>>("Failed to get VendorList ", StatusCodes.Status404NotFound);


                return new Response<IList<Vendor>>(vendorList, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<IList<Vendor>>($"Failed to get vendorList Error", StatusCodes.Status500InternalServerError);
            }

        }

        public async Task<Response<Vendor>> GetAllVendorById(string Id)
        {
            try
            {
                Vendor vendor = await _context.Vendor.AsNoTracking().SingleOrDefaultAsync(c => c.UserId == Id);
                if (vendor == null)
                    return new Response<Vendor>("Failed to find Id", StatusCodes.Status404NotFound);
                return new Response<Vendor>(vendor, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<Vendor>($"Failed to get ServiceProvide Error", StatusCodes.Status500InternalServerError);
            }
        }
     
        public async Task<Response<Vendor>> UpdateVendor(Vendor vendor)
        {
            try
            {
                if (vendor == null)
                    return new Response<Vendor>("Request Not Parsable", StatusCodes.Status400BadRequest);
                Vendor vendorEntity = await _context.Vendor.Include(i=>i.Categories)
                                                                                         .ThenInclude(i=>i.Products)          
                                                                                         .AsNoTracking()
                                                                                        .SingleOrDefaultAsync(c => c.Id == vendor.Id);
                if (vendorEntity == null)
                {
                    return new Response<Vendor>("Vendor not found", StatusCodes.Status404NotFound);
                }
                _context.Update(vendor);
                await _context.SaveChangesAsync();
                return new Response<Vendor>(vendor, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new Response<Vendor>($"Failed to Add ServiceProvide Error", StatusCodes.Status500InternalServerError);
            }
        }

      
    }
}
