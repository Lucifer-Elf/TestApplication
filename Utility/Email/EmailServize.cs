using MailKit.Security;
using MimeKit;
using Scriban;
using Serilog;
using Servize.Utility.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servize.Utility.Email
{
    public class EmailServize
    {
        public enum BodyType
        {
            TEXT = 0,
            FILE = 1
        }



        private string HostName = Configuration.GetParameterValue("base.smtp.hostname");

        //private int PortNumber = Configuration.GetParameterValue("base.smtp.portnumber").;

        private int PortNumber = 20;

        private string UserName = Configuration.GetParameterValue("base.smtp.username");

        private string Password = Configuration.GetParameterValue("base.smtp.password");


        bool IsEmailSent = false;

     
        public EmailServize()
        {

        }


       
        public void ConfigureSmtpClient(string hostName, int portNumber, string user = null, string password = null)
        {
            HostName = hostName;
            PortNumber = portNumber;
            UserName = user;
            Password = password;
        }

        public bool SendMessage(string from, string to, string subject, BodyType bodyType, string bodyData, object parameters = null, string[] attachments = null)
        {
            string bodyText;
            if (bodyType == BodyType.FILE)
            {
                bodyText = GetFileData(bodyData);
            }
            else
            {
                bodyText = bodyData;
            }
            if (bodyText == null)
                return false;

            bodyText = ResolvePlaceHolderScribanTemplate(bodyText, parameters);

            MimeMessage message = CreateMimeMessage(from, to, subject, bodyText, attachments);
            return SendMail(message);
        }


        public static string GetFileData(string emailTemplatePath)
        {
            try
            {
                StreamReader sr = new StreamReader(emailTemplatePath.ToString());
                string bodyText = sr.ReadToEnd();
                return bodyText;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Error on reading File Data");
            }
            return null;
        }

        public static string ResolvePlaceHolder(string bodyText, Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return bodyText;
            }

            string pattern = "\\${(.*?)}";
            var regex = new Regex(pattern, RegexOptions.Compiled);
            Match match = regex.Match(bodyText);

            bodyText = regex.Replace(bodyText, match => parameters.ContainsKey(match.Groups[1].Value) ? parameters[match.Groups[1].Value] : match.Value);

            return bodyText;
        }

        public static string ResolvePlaceHolderScribanTemplate(string bodyText, object parameters)
        {
            if (parameters == null)
            {
                return bodyText;
            }

            var template2 = Template.Parse(bodyText);
            return template2.Render(parameters);
        }

        private static bool ContainsHTML(string checkString)
        {
            return Regex.IsMatch(checkString, "<(.|\n)*?>");
        }


        private static MimeMessage CreateMimeMessage(string from, string to, string subject, string body, string[] attachments)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder();

            if (ContainsHTML(body))
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }

            foreach (string attachment in attachments ?? Enumerable.Empty<string>())
            {
                builder.Attachments.Add(attachment);
            }

            message.Body = builder.ToMessageBody();
            return message;
        }

       
        public bool GetEmailSentStatus()
        {
            return IsEmailSent;
        }

     
        private bool SendMail(MimeMessage message)
        {
            using (var client = new SMSClient())
            {
                //This is for test so no validation
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                if (CanConnect(client) && Authenticate(client))
                {
                    try
                    {
                        client.Send(message);
                        IsEmailSent = true;
                        client.Disconnect(true);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex, "Error on Sending Mail");
                    }
                }
            }
            return false;
        }


        private bool CanConnect(SMSClient client)
        {
            try
            {
                client.Connect(HostName, PortNumber, SecureSocketOptions.Auto);
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error on Sending Mail");
            }
            return false;
        }

        private bool Authenticate(SMSClient client)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                Log.Logger.Information("UserName Empty on Authentication");
                return true;
            }

            try
            {
                client.Authenticate(UserName, Password);
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while Authenticating");
            }
            return false;
        }
    }
}

