using Servize.Utility.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Servize.Utility
{
    public class SMSAuthService
    {
        public static async Task<int> SendTokenSMSAsync(long phoneNumber)
        {
            try
            {
                Random rand = new Random();
                int otp = rand.Next(1000, 9999);
                string message = $"Hello! OTP Verification Code is {otp} for Servize.com";
                await SendSMS(phoneNumber, message);
                return otp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        private static async Task SendSMS(long phoneNumber, string message)
        {
             string account_id =  AzureVault.GetValue("TwilioId");
             string auth_token = AzureVault.GetValue("TwilioToken");

            var to = new PhoneNumber($"+{phoneNumber}");
            var from = new PhoneNumber("+16193502531");
            TwilioClient.Init(account_id, auth_token);
            await MessageResource.CreateAsync(to: to,
                                                from: from,
                                                body: message);

        }
    }
}
