using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace Servize.Utility.Email
{
    public class SMSClient : SmtpClient
    {


        protected override DeliveryStatusNotification? GetDeliveryStatusNotifications(MimeMessage message, MailboxAddress mailbox)
        {
            return DeliveryStatusNotification.Failure | DeliveryStatusNotification.Success;
        }
    }
}
