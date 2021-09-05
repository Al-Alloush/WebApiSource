using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.AppServices
{
    public class AppSettingsService
    {
        private readonly IConfiguration _config;


        public AppSettingsService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// get the Default user name from appsettings json file 
        /// </summary>
        /// <returns>Default Role name</returns>
        public string DefaultUserRole()
        {
           return _config["Settings:DefaultUserRole"];
        }

        /// <summary>
        /// true if a accepted to clock account if login failed more than many time, else false
        /// </summary>
        /// <returns></returns>
        /// 
        public bool LockoutAccountAlloued()
        {
            return Convert.ToBoolean(_config["Settings:PasswordFailuresAcceptLockoutAccount"]);
        }

        public string GetDefaultAppLanguage()
        {
            return _config["Settings:DefaultAppLanguage"];
        }





        /// <summary>
        /// Ensure that the age of the registrant is less than the allowed age
        /// </summary>
        /// <returns>If allowed age 18, and register age 16, return true, else: return false </returns> 
        /// 
        public bool CheckIfAgeEligibleToRegister(DateTime birthday)
        {
            byte eligibleAge;
            if (!byte.TryParse(_config["Settings:EligibleAgeToRegister"], out eligibleAge))
                return false;

            // check if user age is allowed to register
            DateTime today = DateTime.Today;
            DateTime _birthday = (DateTime)birthday;
            int age = today.Year - _birthday.Year;
            if (_birthday > today.AddYears(-age))
                age--;

            if (age < eligibleAge)
                return true;

            // age under eligibleAge
            return false;

        }
    }
}
