using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servize.Utility.Cors
{
    public class CustomCorsPolicyProvider : ICorsPolicyProvider
    {

        public static Dictionary<string, ICorsPolicyProvider> Providers { get; set; } = new Dictionary<string, ICorsPolicyProvider>();


        public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            if (string.IsNullOrEmpty(policyName) || !Providers.ContainsKey(policyName))
            {
                return GetDefaultPolicy();
            }

            var provider = Providers[policyName];

            return await provider.GetPolicyAsync(context, policyName);
        }

        private CorsPolicy GetDefaultPolicy()
        {
            return new CorsPolicyBuilder()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin()
              .Build();
        }
    }
}
