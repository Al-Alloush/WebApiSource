using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.AppService
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);

        string GetCurrentUserEmail();

        string GetCurrentUserId();

        string GetCurrentUserName();
    }
}
