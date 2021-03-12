using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Servize.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Utility
{
    public class Utilities
    {
        ServizeDBContext _context;
        public Utilities(ServizeDBContext context)
        {
            _context = context;
        }

        public async Task CompleteTransactionAsync()
        {
           await _context.SaveChangesAsync();
        }

        public static string GetRoleForstring(string role)
        {
            if (role.ToUpper() == "ADMIN")
                return UserRoles.Admin;

            if (role.ToUpper() == "PROVIDER")
                return UserRoles.Provider;
            return UserRoles.Client;
        }
    }

    /*public class AuthenticationCrosPolicy : ICorsPolicyProvider
    {
        public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policy)
        {
            if (policy != "_myWebOrigin") return null;
            string origin = context.Request.Headers["Origin"].FirstOrDefault();

            return new CorsPolicyBuilder()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(origin)
                .Build();
        
        }
    }*/
}
