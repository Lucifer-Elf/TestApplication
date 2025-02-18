﻿using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Servize.Authentication;
using Servize.Domain.Enums;
using Servize.Domain.Model;
using Servize.Domain.Model.VendorModel;
using Servize.DTO;
using Servize.DTO.ADMIN;
using Servize.Utility;
using Servize.Utility.Logging;
using Servize.Utility.Sms;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Servize.Domain.Repositories
{
    public class AccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServizeDBContext _context;
        private readonly ContextTransaction _transaction;
        private readonly TokenValidationParameters _tokenValidationParameter;


        public AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
             ServizeDBContext context, ContextTransaction transaction, TokenValidationParameters tokenValidationParameter)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _transaction = transaction;
            _tokenValidationParameter = tokenValidationParameter;
        }

        public async Task<Response<AuthSuccessResponse>> AddUserToIdentityWithSpecificRoles(RegistrationInputModel model, string role)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return new Response<AuthSuccessResponse>("User with this Email AlreadyExist", StatusCodes.Status409Conflict);
                }
                ApplicationUser user = new()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = model.PhoneNumber,

                };
                Response<AuthSuccessResponse> response = await CreateNewUserBasedOnRole(role, user, password: model.Password, firstName: model.FirstName, lastName: model.LastName);
                if (response.IsSuccessStatusCode())
                {
                    await _transaction.CompleteAsync();
                    return new Response<AuthSuccessResponse>(response.Resource, response.StatusCode);
                }
                return new Response<AuthSuccessResponse>(response.Message, response.StatusCode);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new Response<AuthSuccessResponse>("Error while creating User", StatusCodes.Status500InternalServerError);
            }
        }



        public async Task<Response<AuthSuccessResponse>> AddUserToIdentityWithSpecificRoles(RegistrationInputModelVendor model, string role)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return new Response<AuthSuccessResponse>("User with this Email AlreadyExist", StatusCodes.Status409Conflict);
                }
                ApplicationUser user = new()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = model.PhoneNumber,

                };
                Response<AuthSuccessResponse> response = await CreateNewUserBasedOnRole(role, user, password: model.Password, companyName: model.CompanyName, companyRegistrationNumber: model.CompanyRegistrationNumber);
                if (response.IsSuccessStatusCode())
                {
                    await _transaction.CompleteAsync();
                    return new Response<AuthSuccessResponse>(response.Resource, response.StatusCode);
                }
                return new Response<AuthSuccessResponse>(response.Message, response.StatusCode);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new Response<AuthSuccessResponse>("Error while creating User", StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<Response<AuthSuccessResponse>> CreateNewUserBasedOnRole(string role, ApplicationUser user, string password, string firstName = "", string lastName = "",
                                                                                    string companyName = "", string companyRegistrationNumber = "")
        {
            try
            {
                var createUser = await _userManager.CreateAsync(user, password);

                if (createUser == null)
                {
                    return new Response<AuthSuccessResponse>("Unable to create User", StatusCodes.Status500InternalServerError);
                }

                if (!createUser.Succeeded)
                {

                    return new Response<AuthSuccessResponse>(createUser.Errors.Select(x => x.Description).ToString(), StatusCodes.Status500InternalServerError);
                }

                await CreateRoleInDatabase();

                if (await _roleManager.RoleExistsAsync(Utility.Utilities.GetRoleForstring(role)))
                {
                   var result =  await _userManager.AddToRoleAsync(user, Utility.Utilities.GetRoleForstring(role));
                    if(!result.Succeeded)
                        return new Response<AuthSuccessResponse>("error while creating Roles for User", StatusCodes.Status500InternalServerError);
                }


                if (Utility.Utilities.GetRoleForstring(role).ToUpper() == "VENDOR")
                {
                    Vendor vendor = new()
                    {
                        UserId = user.Id,
                        CompanyName = companyName,
                        CompanyRegistrationNumber = companyRegistrationNumber,

                    };
                    _context.Add(vendor);
                }
                else
                {
                    Client client = new()
                    {
                        UserId = user.Id,
                        FirstName = firstName,
                        LastName = lastName,
                    };
                    _context.Add(client);
                }


                var response = await GenrateAuthenticationTokenForUser(user);
                if (response.IsSuccessStatusCode())
                    return new Response<AuthSuccessResponse>(response.Resource, response.StatusCode);

                return new Response<AuthSuccessResponse>(response.Message, response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<AuthSuccessResponse>("error while creating new User", StatusCodes.Status500InternalServerError);
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
            if (!await _roleManager.RoleExistsAsync(UserRoles.Vendor))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Vendor));
        }

        public async Task<Response<int>> SendSMSTokenAsync(string phoneNumber)
        {
            try
            {
                string Phno = phoneNumber;
                if (!Phno.StartsWith("+"))
                    Phno = "+" + Phno;
                phoneNumber = Phno;
                var value = await SMSAuthService.SendTokenSMSAsync(phoneNumber);
                return new Response<int>(value, StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<int>("Error while Sending SMS", StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<Response<AuthSuccessResponse>> GenrateAuthenticationTokenForUser(ApplicationUser user)
        {
            try
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Count <= 0)
                {
                    return new Response<AuthSuccessResponse>("Error while Finding Roles for User", StatusCodes.Status500InternalServerError);
                }

                var authSignInKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("RandomKeyIsvalid1234"));
                var tokendescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {

                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Role,userRoles.SingleOrDefault<string>()),
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

                var succesResponse = new AuthSuccessResponse
                {
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken.Token,

                };
                return new Response<AuthSuccessResponse>(succesResponse, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new Response<AuthSuccessResponse>("Error while genrating Token", StatusCodes.Status500InternalServerError);

            }
        }

        public async Task<Response<AuthSuccessResponse>> RefreshTokenAsync(string token, string refreshToken)
        {
            try
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
                var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

                Response<AuthSuccessResponse> response = await GenrateAuthenticationTokenForUser(user);
                if (response.IsSuccessStatusCode())
                {
                    await _transaction.CompleteAsync();
                    return new Response<AuthSuccessResponse>(response.Resource, response.StatusCode);
                }
                return new Response<AuthSuccessResponse>(response.Message, response.StatusCode);


            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new Response<AuthSuccessResponse>("Error while refresh token", StatusCodes.Status500InternalServerError);
            }
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

        public async Task<Response<AuthSuccessResponse>> HandleLoginRequest(InputLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new Response<AuthSuccessResponse>("user Doesnt Exist", StatusCodes.Status404NotFound);

                }
                Response<AuthSuccessResponse> response = await GenrateAuthenticationTokenForUser(user);
                return new Response<AuthSuccessResponse>(response.Resource, StatusCodes.Status200OK);


            }
            return new Response<AuthSuccessResponse>("error while Login", StatusCodes.Status500InternalServerError);

        }


        public async Task<Response<InputLoginModel>> HandleChangedPassword(ChangePasswordRequest changepassword)
        {
            try
            {
                var userExist = await _userManager.FindByIdAsync(changepassword.UserId);
                if (userExist == null)
                    return new Response<InputLoginModel>("No able to find User of specific Id", StatusCodes.Status404NotFound);

                var passWordMatch = await _userManager.CheckPasswordAsync(userExist, changepassword.OldPasword);

                if (!passWordMatch)
                    return new Response<InputLoginModel>("Old Password is not correct", StatusCodes.Status406NotAcceptable);

                var isNewPassWord = await _userManager.ChangePasswordAsync(userExist, changepassword.OldPasword, changepassword.NewPassword);

                if (isNewPassWord.Succeeded)
                    return new Response<InputLoginModel>("Password Changed Successfully", StatusCodes.Status200OK);

                return new Response<InputLoginModel>(isNewPassWord.Errors.ToString(), StatusCodes.Status406NotAcceptable);
            }
            catch
            {
                return new Response<InputLoginModel>("Error while Changing Password", StatusCodes.Status500InternalServerError);
            }
        }



        private async Task<Response<AuthSuccessResponse>> Register(RegistrationInputModel model)
        {
            return await AddUserToIdentityWithSpecificRoles(model, model.Role);
        }

        public async Task<Response<AuthSuccessResponse>> HandleGoogleLogin(GoogleLoginRequest externalInputModel)
        {
            Logger.LogInformation(0, "Google login service started");
            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(externalInputModel.TokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;

                var user = await _userManager.FindByLoginAsync(externalInputModel.Provider, payload.Subject);

                if (user != null)
                    return new Response<AuthSuccessResponse>("user already exist", StatusCodes.Status409Conflict);

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
                    return new Response<AuthSuccessResponse>(result.Errors.ToString(), statusCode: StatusCodes.Status500InternalServerError);


                var userRoles = await _userManager.GetRolesAsync(user);
                Response<AuthSuccessResponse> response = await GenrateAuthenticationTokenForUser(user);
                if (response.IsSuccessStatusCode())
                    return new Response<AuthSuccessResponse>(response.Resource, response.StatusCode);

                return new Response<AuthSuccessResponse>(response.Message, response.StatusCode);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new Response<AuthSuccessResponse>("Error while Google Login", StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.LogInformation(0, "Google login service finished");
            }
        }
    }
}
