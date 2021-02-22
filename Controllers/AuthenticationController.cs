﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Model.Account;
using Servize.Domain.Model.Provider;
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
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeDBContext _context;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
             SignInManager<ApplicationUser> signInManager,
             ServizeDBContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _context = context;
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
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
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
        [Route("UserTokenExpire")]
        public async Task<ActionResult> GetUserTokenValidity([FromBody] ServizeLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
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
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] ServizeLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken");

                    //var isValid = await _userManager.VerifyUserTokenAsync(user, "ServizeApp", "RefreshToken", refreshToken);
                    if (refreshToken != null)
                    {
                        return Ok(new { msg = "AlfredyLogin", refreshToken });
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
                        ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss")

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
        public async Task<IActionResult> LogOut([FromBody] ServizeLoginModel model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
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
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error On LogOut ", StatusCodes.Status500InternalServerError));
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

                var userExist = await _userManager.FindByNameAsync(model.UserName);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response("User Already exist", StatusCodes.Status500InternalServerError));
                }
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response(result.ToString(), StatusCodes.Status500InternalServerError));

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
                        CompanyName = model.CompanyName,
                        EmiratesIdNumber = model.EmiratesIdNumber

                    };
                    _context.ServizeProvider.Add(provider);
                    await _context.SaveChangesAsync();   /// check retun with 0 or less and error return to main
                }

                return Ok(new Response("Admin is Created Sucessfully", StatusCodes.Status201Created));


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response($"Error : {ex.Message}", StatusCodes.Status500InternalServerError));
            }
        }


        private void RedirectToLoginAfterRegister(ServizeUserModel model)
        {
            CreatedAtRoute(nameof(Login), new
            {
                UserName = model.UserName,
                Password = model.Password,
                RememberMe = false
            }, model);
        }

        [HttpGet]
        [Route("google-login")]
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



        // input will be google, 
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {

            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties =  _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);

        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
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

        }
    }
}