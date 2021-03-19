using Google.Apis.Auth;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using Servize.Utility.Logging;
using Servize.Utility.QueryFilter;
using Servize.Utility.Sms;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Servize.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeDBContext _context;
        private readonly IConfiguration _configuration;
        private IAuthService _authService;
        private readonly ContextTransaction _transaction;
        private readonly TokenValidationParameters _tokenValidationParameter;

        public AccountController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
             SignInManager<ApplicationUser> signInManager,
             ServizeDBContext context,
             IAuthService service, ContextTransaction transaction,
             TokenValidationParameters tokenValidationParameter
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _context = context;
            _authService = service;
            _transaction = transaction;
            _tokenValidationParameter = tokenValidationParameter;
        }

        /// <summary>
        /// Register Client
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Authentication/RegisterUser
        [HttpPost("RegisterClient")]
        public async Task<ActionResult> RegisterUser([FromBody] RegistrationInputModel model)
        {
            Response<AuthSuccessResponse> Response = await AddUserToIdentityWithSpecificRoles(model, "CLIENT");
            if (Response.IsSuccessStatusCode())
            {
                return Ok(Response.Resource);
            }
            return Problem(statusCode: Response.StatusCode, detail: Response.Message);
        }


        /// <summary>
        /// RegisterAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Authentication/RegisterAdmin
        [HttpPost("RegisterAdmin")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> RegisterAdmin([FromBody] RegistrationInputModel model)
        {
            Response<AuthSuccessResponse> Response = await AddUserToIdentityWithSpecificRoles(model, "ADMIN");
            if (Response.IsSuccessStatusCode())
            {
                return Ok(Response.Resource);
            }
            return Problem(statusCode: Response.StatusCode, detail: Response.Message);
        }


        /// <summary>
        /// Register provider
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Authentication/RegisterProvider
        [HttpPost]
        [Route("RegisterProvider")]
        public async Task<ActionResult> RegisterServizeProvider([FromBody] RegistrationInputModel model)
        {
            Response<AuthSuccessResponse> Response = await AddUserToIdentityWithSpecificRoles(model, "PROVIDER");
            if (Response.IsSuccessStatusCode())
            {
                return Ok(Response.Resource);
            }
            return Problem(statusCode: Response.StatusCode, detail: Response.Message);
        }

        /* /// <summary>
         /// Get token for User
         /// </summary>
         /// <param name="model"></param>
         /// <returns></returns>
         //Authentication/UserToken
         [HttpGet("UserToken")]
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
                     if (refreshToken != null)
                     {
                         return Ok(refreshToken);
                     }
                 }
             }
             return Unauthorized();
         }*/
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Response<AuthSuccessResponse>>> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {

            Response<AuthSuccessResponse> response = await RefreshTokenAsync(refreshRequest.Token, refreshRequest.RefreshToken);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);


            return Problem(detail: response.Message, statusCode: response.StatusCode);
        }

        private async Task<Response<AuthSuccessResponse>> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return new Response<AuthSuccessResponse>(new AuthSuccessResponse { Errors = new[] { "Invalid token" } }, StatusCodes.Status500InternalServerError);
            }
            var expiryDate = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expirtDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDate);

            if (expirtDateTimeUtc > DateTime.UtcNow)
            {
                return new Response<AuthSuccessResponse>(new AuthSuccessResponse { Errors = new[] { "Token Not expired" } }, StatusCodes.Status500InternalServerError);
            }


            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _context.RefreshToken.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new Response<AuthSuccessResponse>(new AuthSuccessResponse { Errors = new[] { "This RefreshToken Doesnt Exist" } }, StatusCodes.Status500InternalServerError);
            }
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new Response<AuthSuccessResponse>(new AuthSuccessResponse { Errors = new[] { "Refresh token is expired" } }, StatusCodes.Status500InternalServerError);

            }

            if (storedRefreshToken.Invalidated)
                return new Response<AuthSuccessResponse>(new AuthSuccessResponse { Errors = new[] { "RefreshToken has been invalidated" } }, StatusCodes.Status500InternalServerError);

            if (storedRefreshToken.Used)
                return new Response<AuthSuccessResponse>(new AuthSuccessResponse { Errors = new[] { "RefreshToken has been used" } }, StatusCodes.Status500InternalServerError);

            if (storedRefreshToken.JwtId != jti)
                return new Response<AuthSuccessResponse>(new AuthSuccessResponse { Errors = new[] { "Refresh token does not match JWT" } }, StatusCodes.Status500InternalServerError);

            storedRefreshToken.Used = true;
            _context.RefreshToken.Update(storedRefreshToken);
            await _transaction.CompleteAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenrateAuthenticationTokenForUser(user);
        }

        
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameter, out var validatedToken);
                if (!isJwtWithValidSecurityAlogorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }

        }

        private bool isJwtWithValidSecurityAlogorithm(SecurityToken validateToken)
        {

            return (validateToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);



        }


        /// <summary>
        /// Getotp token for number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpPost("GetOtp/{number}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> SMSToken(string number)
        {
            if (User != null)
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var user = _userManager.GetUserAsync(User);
                    if (user.Result != null)
                    {
                        string Phno = user.Result.PhoneNumber;
                        if (!Phno.StartsWith("+"))
                            Phno = "+" + Phno;
                        number = Phno;
                    }
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


        /// <summary>
        /// Verify SmS token
        /// </summary>
        /// <param name="model"> RegistrationModel With all Detail</param>
        /// <returns></returns>

        [HttpPost("VerifyOtp")]
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

        //Authentication/Login
        [HttpPost("Login")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login([FromBody] InputLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Problem(detail: "User Doesnt Exist", statusCode: StatusCodes.Status404NotFound);
                }
                Response<AuthSuccessResponse> response = await GenrateAuthenticationTokenForUser(user);
                if (response.IsSuccessStatusCode())
                    return Ok(response.Resource);
                return Problem(detail: response.Message, statusCode: response.StatusCode);
            }

            return Problem(detail: "error while Login", statusCode: StatusCodes.Status500InternalServerError);
        }

        private async Task<Response<AuthSuccessResponse>> GenrateAuthenticationTokenForUser(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authSignInKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {

                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Role,userRoles.FirstOrDefault<string>()),
                new Claim("id",user.Id)
                }),
                Expires = DateTime.UtcNow.AddSeconds(45),
                SigningCredentials = new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)

            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokendescriptor);

            var refreshToken = new RefreshToken
            {                
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };
            await _context.RefreshToken.AddAsync(refreshToken);
            await _transaction.CompleteAsync();

            var succesResponse = new AuthSuccessResponse
            {
                Token = tokenHandler.WriteToken(token),               
                RefreshToken = refreshToken.Token,

            };
            return new Response<AuthSuccessResponse>(succesResponse, StatusCodes.Status200OK);
        }

        //Authentication/Logout
        [HttpPost("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok("logout Sucessfully");
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
        private async Task<Response<AuthSuccessResponse>> AddUserToIdentityWithSpecificRoles(RegistrationInputModel model, string role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    new Response<AuthSuccessResponse>(new AuthSuccessResponse
                    {
                        Errors = ModelState.Values.SelectMany(i => i.Errors.Select(xx => xx.ErrorMessage))
                    }, StatusCodes.Status500InternalServerError);
                }
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return new Response<AuthSuccessResponse>("User with this Email AlreadyExist", StatusCodes.Status409Conflict);
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
                Logger.LogError(ex);
                return new Response<AuthSuccessResponse>("Error while creating User", StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<Response<AuthSuccessResponse>> CreateNewUserBasedOnRole(RegistrationInputModel model, string role, ApplicationUser user)
        {

            var createUser = await _userManager.CreateAsync(user, model.Password);

            if (!createUser.Succeeded)
            {

                return new Response<AuthSuccessResponse>(createUser.Errors.Select(x => x.Description).ToString(), StatusCodes.Status500InternalServerError);
            }

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

          return await GenrateAuthenticationTokenForUser(user);
            
        }



        /// <summary>
        /// Get User by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

       /* [HttpGet]
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
        }*/

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="changepassword"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
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



        /// <summary>
        /// Google login 
        /// </summary>
        /// <param name="externalInputModel"></param>
        /// <returns></returns>
        [HttpPost("Googlelogin")]       
        public async Task<ActionResult<Response<AuthSuccessResponse>>> Google([FromBody] ExternalLoginDTO externalInputModel)
        {
            Logger.LogInformation(0, "Google login service started");
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
                if (!result.Succeeded)
                    return Problem(detail: result.Errors.ToString(), statusCode: StatusCodes.Status500InternalServerError);


                var userRoles = await _userManager.GetRolesAsync(user);
                Response<AuthSuccessResponse> response = await GenrateAuthenticationTokenForUser(user);
                if (response.IsSuccessStatusCode())
                    return Ok(response.Resource);
                return Problem(detail: response.Message, statusCode: response.StatusCode);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return BadRequest();
            }
            finally
            {
                Logger.LogInformation(0, "Google login service finished");
            }

        }



        [HttpGet]
        [Route("AllAccoutHolder")]
        public async Task<ActionResult<IList<IdentityUser>>> GetListofAllAccountHolder([FromQuery] Query query)
        {
            IQueryable<IdentityRole> identityRole = _roleManager.Roles.ApplyQuery(query);
            IList<IdentityRole> identityRoleList = await identityRole.ToListAsync();
            IList<ApplicationUser> usersList = await _userManager.Users.ToListAsync();

            return Ok((from r in identityRole join u in usersList on r.Id equals u.Id select u).Distinct());

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

    }

}

