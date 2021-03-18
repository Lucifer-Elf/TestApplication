using Google.Apis.Auth;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Model;
using Servize.Domain.Model.Provider;
using Servize.DTO;
using Servize.DTO.ADMIN;
using Servize.Utility;
using Servize.Utility.Sms;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Servize.Controllers
{
    [EnableCors("MyPolicy")]
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
        private readonly ContextTransaction _transaction;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
             SignInManager<ApplicationUser> signInManager,
             ServizeDBContext context,
             IAuthService service, ContextTransaction transaction
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _context = context;
            _authService = service;
            _transaction = transaction;
        }


        //Authentication/RegisterUser
        [HttpPost]
        [Route("RegisterClient")]
        public async Task<ActionResult> RegisterUser([FromBody] RegistrationInputModel model)
        {
            await AddUserToIdentityWithSpecificRoles(model, "CLIENT");
            var LoginModel = new InputLoginModel
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };
            return await Login(LoginModel);
        }


        //Authentication/RegisterAdmin
        [HttpPost]
        [Route("RegisterAdmin")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> RegisterAdmin([FromBody] RegistrationInputModel model)
        {
            await AddUserToIdentityWithSpecificRoles(model, "ADMIN");
            var LoginModel = new InputLoginModel
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };
            return await Login(LoginModel);
        }

        //Authentication/RegisterProvider
        [HttpPost]
        [Route("RegisterProvider")]
        public async Task<ActionResult> RegisterServizeProvider([FromBody] RegistrationInputModel model)
        {
            await AddUserToIdentityWithSpecificRoles(model, "PROVIDER");
            var LoginModel = new InputLoginModel
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };
            return await Login(LoginModel);
        }

        //Authentication/UserToken
        [HttpGet]
        [Route("UserToken")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> GetUserToken([FromBody] InputLoginModel model)
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
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> GetUserTokenValidity([FromBody] InputLoginModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken");


                if (refreshToken != null)
                {
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
                    if (token.ValidTo > DateTime.Now)
                        return Ok(new
                        {
                            isValid = false
                        });
                    return Ok(new
                    {
                        isValid = true,
                        expiryDate = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss")
                    }); ;
                }
            }

            return Unauthorized();
        }

        [HttpPost("Getotp/{number}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> SMSToken(string number)
        {
            if (User != null)
            {
                _signInManager.IsSignedIn(User);
                var user = _userManager.GetUserAsync(User);
                if (user.Result != null)
                {
                    string Phno = user.Result.PhoneNumber;
                    if (!Phno.StartsWith("+"))
                        Phno = "+" + Phno;
                    number = Phno;
                }
            }
            try
            {
                var value = await SMSAuthService.SendTokenSMSAsync(number);
                HttpContext.Session.SetInt32("otp", value);
                return Ok($"Otp Send Sucessfully => {value}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Problem(detail: "Error while Sending SMS", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("Verifyotp")]
        public async Task<ActionResult> VerifySMSToken(RegistrationInputModel model)
        {
            try
            {
                if (HttpContext.Session.GetInt32("otp") == model.Otp)
                {
                    HttpContext.Session.SetInt32("otp", 0);
                    return await Register(model);
                }
                return Problem(detail: "TryAgain", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Problem(detail: "Error While Verifying Otp", statusCode: StatusCodes.Status500InternalServerError);
            }


        }

        private async Task<ActionResult> Register(RegistrationInputModel model)
        {
            if (!string.IsNullOrEmpty(model.Role))
            {
                if (model.Role.ToUpper() == "CLIENT")
                    return await RegisterUser(model);
                if (model.Role.ToUpper() == "ADMIN")
                    return await RegisterAdmin(model);
                else
                    return await RegisterServizeProvider(model);
            }
            else
            {
                return await RegisterUser(model);
            }
        }



        //Authentication/Login
        [HttpPost]
        [Route("Login")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login([FromBody] InputLoginModel model)
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
                        return Ok(new { msg = "AlreadyLogin", RefreshToken = refreshToken });
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
                    var tokenHolder = new TokenHolder
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss"),
                        UserId = user.Id,
                        UserName = user.UserName

                    };
                    await _userManager.SetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken", tokenHolder.Token);

                    return Ok(tokenHolder);
                }
            }
            return Unauthorized();
        }

        //Authentication/Logout
        [HttpPost]
        [Route("Logout")]
        public async Task<ActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
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
            if (!await _roleManager.RoleExistsAsync(UserRoles.Client))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Client));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Provider))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Provider));
        }
        private async Task<ActionResult> AddUserToIdentityWithSpecificRoles(RegistrationInputModel model, string role)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(model.Email);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new Response("User Already exist", StatusCodes.Status409Conflict));
                }
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = model.PhoneNumber,

                };
                return await CreateNewUserBasedOnRole(model, role, user);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error while creating User", StatusCodes.Status500InternalServerError));
            }
        }

        private async Task<ActionResult> CreateNewUserBasedOnRole(RegistrationInputModel model, string role, ApplicationUser user)
        {

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response(result.ToString(), StatusCodes.Status500InternalServerError));

            await CreateRoleInDatabase();

            if (await _roleManager.RoleExistsAsync(Utility.Utilities.GetRoleForstring(role)))
            {
                await _userManager.AddToRoleAsync(user, Utility.Utilities.GetRoleForstring(role));
            }


            if (Utility.Utilities.GetRoleForstring(role) == "Provider")
            {
                Provider provider = new Provider
                {
                    UserId = user.Id,
                    CompanyName = model.CompanyName,
                    CompanyRegistrationNumber = model.CompanyRegistrationNumber,
                 
                };
                _context.Add(provider);
            }
            else
            {
                Client client = new Client
                {
                    UserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                 
                };
                _context.Add(client);
            }
            await _transaction.CompleteAsync();
            return Ok(new Response("Profile Created Sucessfully", StatusCodes.Status201Created));
        }





        [HttpGet]
        [Route("LoginData/{id}")]
        public async Task<ActionResult<InputLoginModel>> GetUserDataById(string Id)
        {
            try
            {
                var userExist = await _userManager.FindByIdAsync(Id);
                if (userExist == null)
                    return Problem(detail: "No able to find User of specific Id", statusCode: StatusCodes.Status404NotFound);

                InputLoginModel model = new InputLoginModel
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
        public async Task<ActionResult<InputLoginModel>> ChangePassword(UserChangePassWordDTO changepassword)
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
        [Route("googlelogin")]
        public async Task<ActionResult> Google([FromBody] ExternalLoginDTO externalInputModel)
        {
            try
            {

                var payload = GoogleJsonWebSignature.ValidateAsync(externalInputModel.TokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;

                var user = await _userManager.FindByLoginAsync(externalInputModel.Provider, payload.Subject);

                if (user != null)
                    return Ok(user);

                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {

                    RegistrationInputModel register = new RegistrationInputModel
                    {
                        Email = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Role = externalInputModel.Role
                    };
                    await Register(register);


                }
                var info = new UserLoginInfo(externalInputModel.Provider, payload.Subject, externalInputModel.Provider.ToUpperInvariant());
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
                    var tokenHolder = new TokenHolder
                    {

                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss"),
                        UserId = user.Id,
                        UserName = user.UserName
                    };
                    await _userManager.SetAuthenticationTokenAsync(user, "ServizeApp", "RefreshToken", tokenHolder.Token);

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

