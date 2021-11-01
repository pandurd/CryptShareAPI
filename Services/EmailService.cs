using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptShareAPI.Services
{
    public static class EmailService
    {
        public async static void SendEmail (string ToAddress, string subject, string content)
        {
            var bytes = Convert.FromBase64String("Ry5ObHhFQ3BhTlN1cTNBb0U1ald6UWF3Llo0RXBFeVdvc2pkeUt2M2t4R1VyNHlGVHRyc1JCckcyLWdDamQ0dlJFZFU=");

            var decodedString = Encoding.UTF8.GetString(bytes);

            var apiKey = decodedString;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("pandudrangan@live.com", "CryptShare");
            var plainTextContent = content;
            var htmlContent = content;

            var to = new EmailAddress(ToAddress, "User");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
