using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO.ADMIN
{
    public class UserChangePassWordDTO
    {
        public string UserId { get; set; }

        public string OldPasword { get; set; }

        public string NewPassword { get; set; }

    }
}
