using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Servize.Authentication;
using Servize.Domain.Repositories;
using Servize.DTO;
using Servize.DTO.ADMIN;
using Servize.Utility;
using Servize.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Servize.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AccountRepository _repository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AccountRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _repository = repository;
        }

        [HttpPost("registerclient")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Response<AuthSuccessResponse>>> RegisterClient([FromBody] RegistrationInputModel model)
        {
            Response<AuthSuccessResponse> response = await _repository.AddUserToIdentityWithSpecificRoles(model, "CLIENT");
            if (response.IsSuccessStatusCode())
            {
                return Ok(response.Resource);
            }
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPost("registeradmin")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Response<AuthSuccessResponse>>> RegisterAdmin([FromBody] RegistrationInputModel model)
        {
            Response<AuthSuccessResponse> response = await _repository.AddUserToIdentityWithSpecificRoles(model, "ADMIN");
            if (response.IsSuccessStatusCode())
            {
                return Ok(response.Resource);
            }
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPost]
        [Route("registervendor")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Response<AuthSuccessResponse>>> RegisterVendor([FromBody] RegistrationInputModelVendor model)
        {
            Response<AuthSuccessResponse> response = await _repository.AddUserToIdentityWithSpecificRoles(model, "VENDOR");
            if (response.IsSuccessStatusCode())
            {
                return Ok(response.Resource);
            }
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        [HttpPost("refreshtoken")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<Response<AuthSuccessResponse>>> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            Response<AuthSuccessResponse> response = await _repository.RefreshTokenAsync(refreshRequest.Token, refreshRequest.RefreshToken);
            if (response.IsSuccessStatusCode())
            {
                return Ok(response.Resource);
            }
            return Problem(detail: response.Message, statusCode: response.StatusCode);
        }

        /// <summary>
        /// Getotp token for number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost("getotp/{number}")]
        public async Task<ActionResult<Response<AuthSuccessResponse>>> SMSToken(string phoneNumber)
        {

            Response<int> response = await _repository.SendSMSTokenAsync(phoneNumber);
            if (response.IsSuccessStatusCode())
            {
                HttpContext.Session.SetInt32("otp", response.Resource);
                return Ok("Otp send Sucessfully");
            }
            return Problem(statusCode: response.StatusCode, detail: response.Message);
        }

        /// <summary>
        /// Verify SmS token
        /// </summary>
        /// <param name="model"> RegistrationModel With all Detail</param>
        /// <returns></returns>

        [HttpPost("verifyotp")]
        public ActionResult<Response<AuthSuccessResponse>> VerifySMSToken(RegistrationInputModel model)
        {
            try
            {
                if (HttpContext.Session.GetInt32("otp") == model.Otp)
                {
                    HttpContext.Session.SetInt32("otp", 0);
                    return Ok("Code verfied Sucessfully");
                }
                return Problem(detail: "TryAgain", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Problem(detail: "Error While Verifying Otp", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<Response<AuthSuccessResponse>> Register(RegistrationInputModel model)
        {
            return await _repository.AddUserToIdentityWithSpecificRoles(model, model.Role.ToUpper());

        }

        //Authentication/Login
        [HttpPost("login")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login([FromBody] InputLoginModel model)
        {
            Response<AuthSuccessResponse> response = await _repository.HandleLoginRequest(model);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);
            return Problem(detail: response.Message, statusCode: response.StatusCode);
        }

        //Authentication/Logout
        [HttpPost("logout")]
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

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="changepassword"></param>
        /// <returns></returns>
        [HttpPost("changepassword")]
        public async Task<ActionResult<Response<InputLoginModel>>> ChangePassword(ChangePasswordRequest changepassword)
        {
            Response<InputLoginModel> response = await _repository.HandleChangedPassword(changepassword);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);
            return Problem(detail: response.Message, statusCode: response.StatusCode);
        }

        /// <summary>
        /// Google login 
        /// </summary>
        /// <param name="externalInputModel"></param>
        /// <returns></returns>
        [HttpPost("googlelogin")]
        public async Task<ActionResult<Response<AuthSuccessResponse>>> Google([FromBody] GoogleLoginRequest externalInputModel)
        {
            Response<AuthSuccessResponse> response = await _repository.HandleGoogleLogin(externalInputModel);
            if (response.IsSuccessStatusCode())
                return Ok(response.Resource);
            return Problem(detail: response.Message, statusCode: response.StatusCode);
        }

        [HttpGet("accounts/{roleName}")]
        public async Task<ActionResult<IList<IdentityUser>>> GetListofAllAccountHolder(string roleName)
        {
            try
            {
                IList<ApplicationUser> users = await _userManager.Users.ToListAsync();
                List<ApplicationUser> userWithRoles = new List<ApplicationUser>();
                foreach (var user in users)
                {
                    if (user != null && await _userManager.IsInRoleAsync(user, roleName))
                        userWithRoles.Add(user);
                }
                return Ok(userWithRoles);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return Problem(detail: e.Message);
            }
        }
    }
}

