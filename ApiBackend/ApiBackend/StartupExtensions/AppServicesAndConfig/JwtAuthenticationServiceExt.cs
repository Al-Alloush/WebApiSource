using Core.Interfaces.AppService;
using Infrastructure.Services.AppServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBackend.StartupExtensions.AppServicesAndConfig
{
    public static class JwtAuthenticationServiceExt
    {
        public static IServiceCollection AddJwtAuthenticationService(this IServiceCollection services, IConfiguration config)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    /* if we forget to add this, we might as well just leave anonymous authentication on and a user can 
                    send up any old token they want because we would never validate that the signing key is correct.*/
                    ValidateIssuerSigningKey = true,
                    /* tell it about our issue assigning key, we need to do the same encoding we did in TokenService.cs Constractor */
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtAuthentication:Key"])),
                    /* same as our token issuer that we're going to add to our configurations in TokenService.cs Constractor */
                    ValidIssuer = config["JwtAuthentication:Issuer"],
                    /* If we do not want any issuer(example: https://localhost:5001 in appsetting.cs file) to accept any token, we will set a "true" here.*/
                    ValidateIssuer = true,
                    /* It can also have the audience to which the token was issued */
                    ValidateAudience = false
                };
            });

            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
