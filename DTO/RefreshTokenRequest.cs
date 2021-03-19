using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }    
        public string RefreshToken { get; set; }
    }
}
