using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO
{
    public class TokenHolder
    {
        public string Token { get; set; }
        public string ValidTo { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
