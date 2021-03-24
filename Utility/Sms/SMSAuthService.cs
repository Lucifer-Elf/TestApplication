using Servize.Utility.Configurations;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Servize.Utility.Sms
{
    public class SMSAuthService
    {
        public static async Task<int> SendTokenSMSAsync(string phoneNumber)
        {
            try
            {
                Random rand = new Random();
                int otp = rand.Next(10000, 99999);
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

        private static async Task SendSMS(string phoneNumber, string message)
        {
             /*string account_id =   AzureVault.GetValue("TwilioId");
             string auth_token =   AzureVault.GetValue("TwilioToken");*/
           
           
              string account_id =  "ACd519bd096ffd5420f221241cd37918c0";
             string auth_token = "9f4886a1d6056d69f4e5eb6b7a2e8c11";
             
             

            var to = new PhoneNumber($"{phoneNumber}");
            var from = new PhoneNumber("+16193502531");
            TwilioClient.Init(account_id, auth_token);
            await MessageResource.CreateAsync(to: to,
                                                from: from,
                                                body: message);

        }
    }
}
