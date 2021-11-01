using CryptShareAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace CryptShareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PostAsync(EmailNotification emailNotification)
        {
            var bytes = Convert.FromBase64String("Ry5ObHhFQ3BhTlN1cTNBb0U1ald6UWF3Llo0RXBFeVdvc2pkeUt2M2t4R1VyNHlGVHRyc1JCckcyLWdDamQ0dlJFZFU=");

            var decodedString = Encoding.UTF8.GetString(bytes);

            var apiKey = decodedString;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("pandudrangan@live.com", "CryptShare");
            var subject = "Cyrpt Share File share";
           
            var plainTextContent = $"File {emailNotification.FileName} has been shared on the site, Please visit site";
            var htmlContent = $"File {emailNotification.FileName} has been shared on the site, Please visit site";

            foreach (var user in emailNotification.Email)
            {
                var to = new EmailAddress(user, "User");
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var res = await client.SendEmailAsync(msg);
            }
           
            return NoContent();
        }
    }
}
