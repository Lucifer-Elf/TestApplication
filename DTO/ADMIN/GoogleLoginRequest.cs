using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO.ADMIN
{
   
    public class GoogleLoginRequest
    {
        public string TokenId { get; set; }
        public string Provider { get; set; }
        public string Role { get; set; }
    }
}
