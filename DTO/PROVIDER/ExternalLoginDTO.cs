using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.DTO.PROVIDER
{
    public class ExternalLoginDTO
    {
        public string ProviderName { get; set; }
        public string  ReturnUrl { get; set; }
    }
}
