using Core.Entities.Identity;
using Infrastructure.Services.ThirdPartyServices;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.AppServices
{
    public class EmailService
    {
        private readonly EmailSenderService _emailSenderService;
        private readonly UserManager<AppUser> _userManager;

        public EmailService(EmailSenderService  emailSenderService, UserManager<AppUser> userManager)
        {
            _emailSenderService = emailSenderService;
            _userManager = userManager;
        }

        string button = "<div style='width: 100% ; margin-top: 50px; margin-bottom: 50px;' > " +
                            "<a href='{0}' style='width:155px; height:25px; background: #4E9CAF;" +
                                                  "padding: 10px; text-align:center; border-radius:5px;" +
                                                   "color:white; font-weight:bold; line-height:25px;'>" +
                                "{1}" +
                            "</a>" +
                        "</div> ";



        public async Task<bool> SendConfirmEmailAsync(AppUser user, string confirmationLink)
        {
            string emailHeader = "EmcilConfirmEmailHeader" + "<br>";
            string emailBody = string.Format(button, confirmationLink, "Activate");
            string emailFotter = "";
            string email = emailHeader + emailBody + emailFotter;

            var sendEmailResult = await _emailSenderService.SendEmailAsync("EmailConfirmEmailSubject", user.Email, email);
            // if confirm email not sent, delete the user and reten BadRequest
            if (!sendEmailResult)
                return false;

            return true;
        }


        public async Task<bool> SendResetPasswordAsync(AppUser user, string resetPasswordLink)
        {
            string emailHeader = "PasswordResetEmailHeader" + "<br>";
            string emailBody = string.Format(button, resetPasswordLink, "Reset Password");
            string emailFotter = "";
            string email = emailHeader + emailBody + emailFotter;

            var sendEmailResult = await _emailSenderService.SendEmailAsync("PasswordResetEmailSubject", user.Email, email);
            // if confirm email not sent, delete the user and reten BadRequest
            if (!sendEmailResult)
                return false;

            return true;
        }
    }
}
