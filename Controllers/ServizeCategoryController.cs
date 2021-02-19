using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Services;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    public class ServizeCategoryController : ControllerBase
    {
        [Authorize(Roles = UserRoles.Provider + "," + UserRoles.Admin)]
        [ApiController]
        [Route("[controller]")]
        public class ServizeProviderController : ControllerBase
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;          
            private readonly ServizeCategoryService _services;

            public ServizeProviderController(ServizeDBContext dbContext,
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
        }
    }
}
