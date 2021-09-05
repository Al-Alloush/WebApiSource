using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Infrastructure.Services.ThirdPartyServices
{
    public class SmsSenderService
    {
        private readonly IConfiguration _config;
        private string _SendSmsAccountSID;
        private string _SendSmsAuthToken;
        private string _SendSmsNumber;
        public SmsSenderService(IConfiguration config)
        {
            _config = config;
            _SendSmsAccountSID = _config["SendSmsTwilioAPI:AccountSID"];
            _SendSmsAuthToken = _config["SendSmsTwilioAPI:AuthToken"];
            _SendSmsNumber = _config["SendSmsTwilioAPI:TwilioPhoneNumber"];
        }


        /// <summary> Send SMS(Twilio API) to mobile number with verification code,  and verification link to verify the phone number directly from mobile</summary>
        /// <returns> true if sent successfully, else return false </returns> 
        public async Task<bool> SmsSender(string phoneNumber, string message)
        {
            try
            {
                // Convert the Number to be valid in Twilio API
                var ToPhoneNumber = ConvertNumberToTwilioFormat(phoneNumber);

                // if phone number is valid
                if (!string.IsNullOrEmpty(ToPhoneNumber))
                {
                    // Initialize the Twilio client
                    TwilioClient.Init(_SendSmsAccountSID, _SendSmsAuthToken);

                    // Send a new outgoing SMS by POSTing to the Messages resource
                    await MessageResource.CreateAsync(
                        // the phone number SMS API Twilio has a special format, it must not contain 00, for that change it to 
                        // From number, must be an SMS-enabled Twilio number
                        from: new PhoneNumber(_SendSmsNumber),
                        to: new PhoneNumber(ToPhoneNumber),
                        // Message content
                        body: message
                    );
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ConvertNumberToTwilioFormat(string phoneNumber)
        {
            string first2 = phoneNumber.Substring(0, 2);
            if (first2 == "00")
            {
                phoneNumber = "+" + phoneNumber.Substring(2);
                return phoneNumber;
            }
            else if (first2.Contains("+"))
                return phoneNumber;

            return null;
        }
    }
}
