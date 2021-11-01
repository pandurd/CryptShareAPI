using CryptShareAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SendGrid;
using SendGrid.Helpers.Mail;

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
            var apiKey = "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";
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
