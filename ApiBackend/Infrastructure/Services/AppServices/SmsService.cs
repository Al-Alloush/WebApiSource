using Core.Entities.Identity;
using Infrastructure.Services.ThirdPartyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.AppServices
{
    public class SmsService
    {
        private readonly SmsSenderService _smsSenderService;

        public SmsService(SmsSenderService smsSenderService)
        {
            _smsSenderService = smsSenderService;
        }



        public async Task<bool> SendConfirmSmsAsync(string phoneNumber, string token, string confirmationLink)
        {
            string message = "PhoneConnfirmSmsMsgBody" + token + "\nor you can click on the next link to confirmed it directly:" + confirmationLink;
            var sendSmsResult = await _smsSenderService.SmsSender(phoneNumber, message);
            if (!sendSmsResult)
                return false;

            return true;
        }
    }
}
