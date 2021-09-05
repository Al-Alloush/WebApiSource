using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SpecificationsQueries.Identity
{

    public class SpeciUser_FindUserByEmail : BaseSpecification<AppUser>
    {
        public SpeciUser_FindUserByEmail(string email, string include = null)  : base( x => x.Email == email )
        {
            if (!string.IsNullOrEmpty(include) && !string.IsNullOrWhiteSpace(include))
                IncludeTables(include);

        }

        private void IncludeTables(string include)
        {
            if (include.ToUpper().Trim().Contains("USERADDRESS"))
                AddInclude(x => x.UserAddress);

            if (include.ToUpper().Trim().Contains("USERIMAGE"))
                AddInclude(x => x.UserImages);
        }
    }

    public class SpeciUser_FindUserByUserName : BaseSpecification<AppUser>
    {
        public SpeciUser_FindUserByUserName(string userName, string include = null) : base(x => x.UserName == userName)
        {
            if (!string.IsNullOrEmpty(include) && !string.IsNullOrWhiteSpace(include))
                IncludeTables(include);

        }

        private void IncludeTables(string include)
        {
            if (include.ToUpper().Trim().Contains("USERADDRESS"))
                AddInclude(x => x.UserAddress);

            if (include.ToUpper().Trim().Contains("USERIMAGE"))
                AddInclude(x => x.UserImages);
        }
    }

    public class SpeciUser_FindUserById : BaseSpecification<AppUser>
    {
        public SpeciUser_FindUserById(string id, string include = null) : base(x => x.Id == id)
        {
            if (!string.IsNullOrEmpty(include) && !string.IsNullOrWhiteSpace(include))
                IncludeTables(include);

        }

        private void IncludeTables(string include)
        {
            if (include.ToUpper().Trim().Contains("USERADDRESS"))
                AddInclude(x => x.UserAddress);

            if (include.ToUpper().Trim().Contains("USERIMAGE"))
                AddInclude(x => x.UserImages);
        }
    }

    public class SpeciUser_FindUserByPhoneNumber : BaseSpecification<AppUser>
    {
        public SpeciUser_FindUserByPhoneNumber(string phoneNumber, string include = null) 
            : base(x => x.PhoneNumber == phoneNumber && x.PhoneNumberConfirmed == true)
        {
            if (!string.IsNullOrEmpty(include) && !string.IsNullOrWhiteSpace(include))
                IncludeTables(include);

        }

        private void IncludeTables(string include)
        {
            if (include.ToUpper().Trim().Contains("USERADDRESS"))
                AddInclude(x => x.UserAddress);

            if (include.ToUpper().Trim().Contains("USERIMAGE"))
                AddInclude(x => x.UserImages);
        }
    }

    

    public class SpeciUser_UserAddress: BaseSpecification<UserAddress>
    {
        public SpeciUser_UserAddress(string userId)
            : base(x=>x.UserId == userId)
        {

        }
    }
}
