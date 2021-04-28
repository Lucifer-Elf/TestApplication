using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Servize.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExternalLoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeDBContext _context;

        public ExternalLoginController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ServizeDBContext context
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;

        }

        [HttpGet]
        [Route("googlelogin")]
        public ActionResult Googlelogin(string returnUrl)
        {
            string redirectUrl = Url.Action("GoogleResponse", "Authentication", null, Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return new ChallengeResult("Google", properties);
        }


        [HttpGet]
        [Route("googleResponse")]
        public async Task<ActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Problem();

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };
            if (result.Succeeded)
                return Ok(userInfo);
            else
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
                };

                IdentityResult identResult = await _userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await _userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return Ok(userInfo);
                    }
                }
                return Problem(statusCode: StatusCodes.Status403Forbidden, detail: "Acess Denied");
            }
        }
    }
}
