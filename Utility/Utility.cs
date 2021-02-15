using Servize.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Utility
{
    public class Utility
    {
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
