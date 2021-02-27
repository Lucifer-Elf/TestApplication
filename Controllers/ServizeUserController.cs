using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Services;
using Servize.Utility;

namespace Servize.Controllers
{
    [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class ServizeUserController : ControllerBase
    {
        //UserClientDTO
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeUserServices _services;

        public ServizeUserController(ServizeDBContext dbContext,
                                         IMapper mapper,
                                         UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        Utilities utitlity
                                         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _services = new ServizeUserServices(dbContext, mapper, utitlity);
        }

    }
}
