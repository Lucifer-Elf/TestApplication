﻿using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
                    var kvUri             = Configuration.GetParameterValue("azure.vault.url").TrimEnd('/');
                    string tenantId       = Configuration.GetParameterValue("azure.vault.tenantid");
                    string clientId       = Configuration.GetParameterValue("azure.vault.clientid");
                    string clientSecret   = Configuration.GetParameterValue("azure.vault.clientsecret");


                    ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                    var secretClient                  = new SecretClient(new System.Uri(kvUri), credential);
                    var secret                        = await secretClient.GetSecretAsync(secretName).ConfigureAwait(false);

                    cache.Add(secretName, secret.Value.Value, new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(1) });
                    return secret.Value.Value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return null;
            }
            return cache[secretName] as string;
        }
    }
}