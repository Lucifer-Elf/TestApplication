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
            return UserRoles.User;
        }
    }
}
