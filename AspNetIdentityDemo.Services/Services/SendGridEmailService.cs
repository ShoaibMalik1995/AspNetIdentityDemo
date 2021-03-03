using AspNetIdentityDemo.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Services.Services
{
    public class SendGridEmailService : IEmailService
    {
        #region Properties
        private readonly IConfiguration Configuration;
        #endregion

        #region Constr
        public SendGridEmailService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        #endregion

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = Configuration["SendGridAPIKey:AuthIdentityDemoKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("skaramat@makeityourweb.com", "Jwt Auth Demo.");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);

        }

    }
}
