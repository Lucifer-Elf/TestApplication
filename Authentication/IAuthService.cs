
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Servize.DTO.ADMIN;

namespace Servize.Authentication
{
    public interface IAuthService
    {
        Task<ApplicationUser> Authenticate(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload);
    }

    public class AuthService : IAuthService
    {
       
        private static IList<ApplicationUser> _users = new List<ApplicationUser>();
        public async Task<ApplicationUser> Authenticate(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            await Task.Delay(1);
            return this.FindUserOrAdd(payload);
        }

        private ApplicationUser FindUserOrAdd(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            var u = _users.Where(x => x.Email == payload.Email).FirstOrDefault();
            if (u == null)
            {
                u = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = payload.Email,
                    Email = payload.Email,                    
                  
                };
                _users.Add(u);
            }
            this.PrintUsers();
            return u;
        }

        private void PrintUsers()
        {
            string s = String.Empty;
            foreach (var u in _users) s += "\n[" + u.Email + "]";
           
        }

       
    }
}