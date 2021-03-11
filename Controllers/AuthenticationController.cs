using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Model.Account;
using Servize.Domain.Model.Client;
using Servize.Domain.Model.Provider;
using Servize.DTO.ADMIN;
using Servize.DTO.PROVIDER;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    [EnableCors("_myWebOrigin")]
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeDBContext _context;
        private readonly IConfiguration _configuration;
        private IAuthService _authService;
        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
             SignInManager<ApplicationUser> signInManager,
             ServizeDBContext context,
             IAuthService service
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _context = context;
            _authService = service;
        }


        //Authentication/RegisterUser
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<ActionResult> RegisterUser([FromBody] ServizeUserModel model)
        {
            return await AddUserToIdentityWithSpecificRoles(model, "USER");
        }


        //Authentication/RegisterAdmin
        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<ActionResult> RegisterAdmin([FromBody] ServizeUserModel model)
        {
            return await AddUserToIdentityWithSpecificRoles(model, "ADMIN");
        }

        //Authentication/RegisterProvider
        [HttpPost]
        [Route("RegisterProvider")]
        public async Task<ActionResult> RegisterServizeProvider([FromBody] ServizeUserModel model)
        {
            return await AddUserToIdentityWithSpecificRoles(model, "PROVIDER");
        }

        //Authentication/UserToken
        [HttpGet]
        [Route("UserToken")]
        public async Task<ActionResult> GetUserToken([FromBody] ServizeLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken");

                    //var isValid = await _userManager.VerifyUserTokenAsync(user, "ServizeApp", "RefreshToken", refreshToken);
                    if (refreshToken != null)
                    {
                        return Ok(refreshToken);
                    }
                }
            }
            return Unauthorized();
        }

        //Authentication/UserTokenExpire
        [HttpGet]
        [Route("UserTokenValidty")]
        public async Task<ActionResult> GetUserTokenValidity([FromBody] ServizeLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken");


                    if (refreshToken != null)
                    {
                        var token = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
                        return Ok(token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss"));
                    }
                }
            }
            return Unauthorized();
        }


        //Authentication/Login
        [HttpPost]
        [Route("login")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login([FromBody] ServizeLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken");

                    //var isValid = await _userManager.VerifyUserTokenAsync(user, "ServizeApp", "RefreshToken", refreshToken);
                    if (refreshToken != null)
                    {
                        return Ok(new { msg = "AlreadyLogin", refreshToken });
                    }
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                     };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddDays(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)

                        );
                    var tokenHolder = new
                    {

                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        validTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss"),
                        userId = user.Id,
                        userName = user.UserName

                    };
                    await _userManager.SetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken", tokenHolder.token);

                    return Ok(tokenHolder);
                }
            }

            return Unauthorized();
        }

        //Authentication/Logout
        [HttpPost]
        [Route("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
              /*  var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {

                        await _userManager.UpdateSecurityStampAsync(user);
                        var resultValue = await _userManager.RemoveAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken");
                        if (resultValue.Succeeded)
                        {
                            return Ok(resultValue);
                        }
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error On LogOut ", StatusCodes.Status500InternalServerError));
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error On LogOut ", StatusCodes.Status500InternalServerError));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error On LogOut ", StatusCodes.Status500InternalServerError));*/
            }

            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error On LogOut ", StatusCodes.Status500InternalServerError));
            }

        }


        // All Private function listed down
        private async Task CreateRoleInDatabase()
        {
            // creating Roles In Database.
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Provider))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Provider));
        }
        private async Task<ActionResult> AddUserToIdentityWithSpecificRoles(ServizeUserModel model, string role)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(model.Email);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response("User Already exist", StatusCodes.Status500InternalServerError));
                }
                ApplicationUser user = new ApplicationUser()
                {
                    UserName= model.Email,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                return await CreateNewUserBasedOnRole(model, role, user);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response($"Error : {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }

        private async Task<ActionResult> CreateNewUserBasedOnRole(ServizeUserModel model, string role, ApplicationUser user)
        {
            
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response(result.ToString(), StatusCodes.Status500InternalServerError));

            string CompanyName = model.CompanyName;
            string EmiratesId = model.EmiratesIdNumber;

            await CreateRoleInDatabase();

            if (await _roleManager.RoleExistsAsync(Utility.Utilities.GetRoleForstring(role)))
            {
                await _userManager.AddToRoleAsync(user, Utility.Utilities.GetRoleForstring(role));
            }

            //RedirectToLoginAfterRegister(model);
            if (Utility.Utilities.GetRoleForstring(role) == "Provider")
            {
                ServizeProvider provider = new ServizeProvider
                {
                    UserId = user.Id,
                    RegistrationDate = DateTime.UtcNow,
                    ModeType = ServizeEnum.ServizeModeType.FIXED,
                    PackageType = ServizeEnum.PackageType.FREE,
                    CompanyName = CompanyName,
                    EmiratesIdNumber = EmiratesId

                };
                _context.Add(provider);
                await _context.SaveChangesAsync();   /// check retun with 0 or less and error return to main
                return Ok(new Response("Provider Added is Created Sucessfully", StatusCodes.Status201Created));
            }
            else
            {
                UserClient client = new UserClient
                {
                    UserId = user.Id,
                };
                _context.Add(client);
                await _context.SaveChangesAsync();
                return Ok(new Response("User Added is Created Sucessfully", StatusCodes.Status201Created));
            }
        }

        private void RedirectToLoginAfterRegister(ServizeUserModel model)
        {
            CreatedAtRoute(nameof(Login), new
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = false
            }, model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            ServizeLoginModel model = new ServizeLoginModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

            };
            return Ok(model);

        }



        [HttpGet]
        [Route("LoginData/{id}")]
        public async Task<ActionResult<ServizeLoginModel>> GetUserDataById(string Id)
        {
            try
            {
                var userExist = await _userManager.FindByIdAsync(Id);
                if (userExist == null)
                    return Problem(detail: "No able to find User of specific Id", statusCode: StatusCodes.Status404NotFound);

                ServizeLoginModel model = new ServizeLoginModel
                {
                    Email = userExist.UserName,

                };
                return Ok(model);
            }
            catch
            {
                return Problem(detail: "Error while getting UserDetaild From Identity", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("ChangePassword")]
        public async Task<ActionResult<ServizeLoginModel>> ChangePassword(UserChangePassWordDTO changepassword)
        {
            try
            {
                var userExist = await _userManager.FindByIdAsync(changepassword.UserId);
                if (userExist == null)
                    return Problem(detail: "No able to find User of specific Id", statusCode: StatusCodes.Status404NotFound);

                var passWordMatch = await _userManager.CheckPasswordAsync(userExist, changepassword.OldPasword);

                if (!passWordMatch)
                    return Problem(detail: "Old Pass is Word", statusCode: StatusCodes.Status406NotAcceptable);

                var isNewPassWord = await _userManager.ChangePasswordAsync(userExist, changepassword.OldPasword, changepassword.NewPassword);

                if (isNewPassWord.Succeeded)
                    return Ok("Password Changed Successfully");

                return Problem(detail: isNewPassWord.Errors.ToString(), statusCode: StatusCodes.Status406NotAcceptable);
            }
            catch
            {
                return Problem(detail: "Error while Changing Password", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("google-login")]
        public async Task<ActionResult> Google([FromBody] DTO.ADMIN.ExternalLoginDTO userView)
        {
            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(userView.TokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;

                var user = await _userManager.FindByLoginAsync(userView.Provider, payload.Subject);

                if (user != null)
                    return Ok(user);
                
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                     user = new ApplicationUser()
                    {
                         UserName= payload.Email,
                        Email = payload.Email,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                
                    await _userManager.CreateAsync(user);
                    await CreateRoleInDatabase();

                    if (await _roleManager.RoleExistsAsync(Utility.Utilities.GetRoleForstring(userView.Role)))
                    {
                        await _userManager.AddToRoleAsync(user, Utility.Utilities.GetRoleForstring(userView.Role));
                    }

                    //RedirectToLoginAfterRegister(model);
                    if (Utility.Utilities.GetRoleForstring(userView.Role) == "Provider")
                    {
                        ServizeProvider provider = new ServizeProvider
                        {
                            UserId = user.Id,
                            RegistrationDate = DateTime.UtcNow,
                            ModeType = ServizeEnum.ServizeModeType.FIXED,
                            PackageType = ServizeEnum.PackageType.FREE,
                            CompanyName = payload.Name,
                            EmiratesIdNumber = "XXXXXXXX"

                        };
                        _context.ServizeProvider.Add(provider);
                       // await _context.SaveChangesAsync();   /// check retun with 0 or less and error return to main
                       // return Ok(new Response("Provider Added is Created Sucessfully", StatusCodes.Status201Created));
                    }
                    else if (Utility.Utilities.GetRoleForstring(userView.Role) == "Admin")
                     {
                          // return Ok(new Response("Admin is Created Sucessfully", StatusCodes.Status201Created));
                     }
                    else
                    {
                        UserClient client = new UserClient
                        {
                            UserId = user.Id,
                        };
                        _context.UserClient.Add(client);
                        //await _context.SaveChangesAsync();
                       // return Ok(new Response("User Added is Created Sucessfully", StatusCodes.Status201Created));
                    }
                }
                var info = new UserLoginInfo(userView.Provider, payload.Subject, userView.Provider.ToUpperInvariant());
                var result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                     var userRoles = await _userManager.GetRolesAsync(user);
                     var authClaims = new List<Claim>
                         {
                         new Claim(ClaimTypes.Name,user.UserName),
                         new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                          };
                     foreach (var userRole in userRoles)
                     {
                         authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                     }
                     var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                     var token = new JwtSecurityToken(
                         issuer: _configuration["JWT:ValidIssuer"],
                         audience: _configuration["JWT:ValidAudience"],
                         expires: DateTime.Now.AddDays(1),
                         claims: authClaims,
                         signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)

                         );
                     var tokenHolder = new
                     {

                         token = new JwtSecurityTokenHandler().WriteToken(token),
                         validTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss"),
                         userId = user.Id,
                         userName = user.UserName

                     };
                     await _userManager.SetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken", tokenHolder.token);

                     return Ok(tokenHolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            return BadRequest();
        }
    }
}


    /* [HttpPost]
       [Route("ExternalLoginCallback")]
       [Produces("application/json")]
       [Consumes("application/json")]
       public async Task<ActionResult> ExternalLoginCallback(string returnUrl, string remoteError = null)
       {

           returnUrl = returnUrl ?? Url.Content("~/");
           ServizeLoginModel loginViewModel = new ServizeLoginModel
           {
               ReturnUrl = returnUrl,
               ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

           };

           if (remoteError != null)
           {

               ModelState.AddModelError(string.Empty, $"Error for external provider:{remoteError}");
               return Problem();
           }

           var info = await _signInManager.GetExternalLoginInfoAsync();
           if (info == null)
           {
               ModelState.AddModelError(string.Empty, "Error loding external information}");
               return Problem();
           }


           // must to have row in aspnetuserslogin 
           var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

           if (signInResult.Succeeded)
           {
               return Ok(returnUrl);
           }
           else
           {
               var email = info.Principal.FindFirstValue(ClaimTypes.Email);
               if (email != null)
               {
                   var user = await _userManager.FindByEmailAsync(email);

                   if (user == null)
                   {

                       user = new ApplicationUser
                       {
                           UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                           Email = info.Principal.FindFirstValue(ClaimTypes.Email),

                       };
                       await _userManager.CreateAsync(user);
                   }

                   await _userManager.AddLoginAsync(user, info);
                   await _signInManager.SignInAsync(user, isPersistent: false);

                   return Ok(returnUrl);
               }
               return Problem("Error with Login");

           }

       }*/


    /* // input will be google, 

     [AllowAnonymous]
     public ActionResult ExternalLogin(string returnUrl = null)
     {
         var redirectUrl = Url.Action("ExternalLoginCallback", "Authentication", new { returnUrl });
         var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
         return new ChallengeResult("Google", properties);

     }

     [HttpPost]
     [Route("facebook-login")]
     [AllowAnonymous]
     public ActionResult ExternalLoginFaceBook(string returnUrl = null)
     {
         var redirectUrl = Url.Action("ExternalLoginCallback", "Authentication", new { returnUrl });
         var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
         return new ChallengeResult("Facebook", properties);

     }*/

