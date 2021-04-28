using System;
using System.Collections.Generic;

namespace Servize.DTO
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public IEnumerable<String> Errors { get; set; }

    }
}
