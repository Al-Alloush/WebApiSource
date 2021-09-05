using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.ThirdPartyServices
{
    public class EmailSenderService
    {
        private readonly IConfiguration _config;
        private string _SendGridAPI_Key;
        private string _SendGridAPI_Email;

        public EmailSenderService(IConfiguration config)
        {
            _config = config;
            _SendGridAPI_Key = _config["SendGridAPI:SendGridApiKey"];
            _SendGridAPI_Email = _config["SendGridAPI:OutputEmail"];
        }

        public async Task<bool> SendEmailAsync(string subject, string toEmail, string _email)
        {
            try
            {
                var _client = new SendGridClient(_SendGridAPI_Key);
                var _from = new EmailAddress(_SendGridAPI_Email);
                var _to = new EmailAddress(toEmail);
                var email = MailHelper.CreateSingleEmail(_from, _to, subject, _email, _email);
                await _client.SendEmailAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
