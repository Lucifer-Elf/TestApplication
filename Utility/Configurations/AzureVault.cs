using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Serilog;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Servize.Utility.Configurations
{
    public class AzureVault
    {
        public static string GetValue(string key)
        {
            if (key.Contains("."))
                key = key.Replace(".", "-");
            return Task.Run(async () => await OnGet(key)).Result;
        }


        private static async Task<string> OnGet(string secretName)
        {
            MemoryCache cache = MemoryCache.Default;
            if (!cache.Contains(secretName))
            {
                try
                {
                    var kvUri             = Configuration.GetValue<string>("azurevaulturl").TrimEnd('/');
                    string tenantId       = Configuration.GetValue<string>("azurevaulttenantid");
                    string clientId       = Configuration.GetValue<string>("azurevaultclientid");
                    string clientSecret   = Configuration.GetValue<string>("azurevaultclientsecret");


                    ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                    var secretClient                  = new SecretClient(new System.Uri(kvUri), credential);
                    var secret                        = await secretClient.GetSecretAsync(secretName).ConfigureAwait(false);

                    cache.Add(secretName, secret.Value.Value, new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(1) });
                    return secret.Value.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    Console.WriteLine(e.Message);
                }
                return null;
            }
            return cache[secretName] as string;
        }
    }
}
