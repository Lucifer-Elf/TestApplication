using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public string Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
