using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Model;
using Servize.Utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        //Authentication/Provider
        [HttpPost]
        [Route("RegisterProvider")]
        public async Task<ActionResult> RegisterServizeProvider([FromBody] ServizeUserModel model)
        {
            return await AddUserToIdentityWithSpecificRoles(model, "PROVIDER");
        }


        //Authentication/Login
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
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
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss")

                    });
                }
            }

            return Unauthorized();          
        }

        //Authentication/Logout
        [HttpPost]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error On LogOut ", StatusCodes.Status500InternalServerError));
            }
            return Ok();
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

                if (await _roleManager.RoleExistsAsync(Utility.Utility.GetRoleForstring(role)))
                {
                    await _userManager.AddToRoleAsync(user, Utility.Utility.GetRoleForstring(role));
                }

                //RedirectToLoginAfterRegister(model);
                if (Utility.Utility.GetRoleForstring(role) == "Provider")
                {
                    ServizeProvider provider = new ServizeProvider
                    {
                        UserId = userExist.Id,
                        RegistrationDate = DateTime.UtcNow,
                        ModeType = ServizeEnum.ServizeModeType.FIXED,
                        PackageType= ServizeEnum.PackageType.FREE
                    };
                    _context.ServizeProvider.Add(provider);
                    await _context.SaveChangesAsync();   /// chck retun with 0 or less and error return to main


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


    }
}