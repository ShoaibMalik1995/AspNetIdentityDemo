using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
