using ApiBackend.ApiErrorHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ApiBackend.StartupExtensions.AppServicesAndConfig
{
    public static class ErrorApiBehaviorServiceExt
    {

        public static IServiceCollection AddErrorApiBehaviorOptions(this IServiceCollection services)
        {

            // Override Api [ApiContoller] Behavior Options to Handling Error: Validation Error (The value 'xx' is not valid.)
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}
