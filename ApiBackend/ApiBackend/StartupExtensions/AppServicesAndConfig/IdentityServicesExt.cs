using Core.Entities.Identity;
using Infrastructure.DataApp;
using Infrastructure.Services;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiBackend.StartupExtensions.AppServicesAndConfig
{
    public static class IdentityServicesExt
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {

            /******************************************************************/
            // entity framework implementation of identity information stores. where we were adding the UserManager to
            // DefaultUsersAsync method in Infrastructure/Identity folder that kind of service is contained inside entity framework stores.
            services.AddIdentityCore<AppUser>() /*Add service for type 'Microsoft.AspNetCore.Identity.UserManager:*/
                    .AddRoles<AppIdentityRole>() /*Add IdentityRole service in Application:*/
                    .AddUserManager<AppUserManager>() // Add AppUserManager
                    .AddRoleManager<AppRoleManager>() // Add AppRoleManager
                    .AddEntityFrameworkStores<AppDbContext>() /*to avoid error :Unable to resolve service for type 'Microsoft.AspNetCore.Identity.IUserStore`1 */
                    .AddSignInManager<SignInManager<AppUser>>() /* to inject SignInManager service need to inject another service*/
                    .AddDefaultTokenProviders() /* to use Microsoft.AspNetCore.Identity Token provider to use function like:UserManager.GenerateEmailConfirmationTokenAsync()*/;
            /********************************************************************/

            services.AddScoped<UserAddressService>();
            services.AddScoped<UserImageService>();

            return services;
        }


    }
}
