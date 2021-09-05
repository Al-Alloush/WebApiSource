using ApiBackend.ApiErrorHandlers;
using ApiBackend.StartupExtensions.AppServicesAndConfig;
using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Infrastructure.DataApp;
using Infrastructure.RepositoriesAndPatterns;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace ApiBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // In Development Mode
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // IdentityDbServer
            services.AddDbContext<AppDbContext>(x =>
                x.UseMySql(
                    $"Server={Configuration["IdentityDbServer"]}; " +
                    $"Database={Configuration["IdentityDatabase"]}; " +
                    $"Uid={Configuration["IdentityDbUid"]}; " +
                    $"Pwd={Configuration["IdentityDbPwd"]}", 
                    mySqlOptionsAction: x => { 
                        x.EnableRetryOnFailure(5); 
                        x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); 
                    }));

            ConfigureServices(services);
        }

        // In Production Mode
        public void ConfigureProductionsServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(x =>
                x.UseMySql(
                     $"Server={Configuration["IdentityDbServer"]}; " +
                    $"Database={Configuration["IdentityDatabase"]}; " +
                    $"Uid={Configuration["IdentityDbUid"]}; " +
                    $"Pwd={Configuration["IdentityDbPwd"]}"
                    ));

            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiBackend", Version = "v1" });
            });

            //RepositoriesAndPatterns
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddRazorPages(); // to serve Razor Pages

            // extentions
            services.AddIdentityServices();
            services.AddJwtAuthenticationService(Configuration);
            services.AddAppAndThirdPartyServices(Configuration);
            services.AddErrorApiBehaviorOptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            //handling exceptions just in developer mode.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiBackend v1"));
            };
            // change app.UseDeveloperExceptionPage(); to custome ExceptionMiddleware 
            app.UseMiddleware<ExceptionMiddleware>();

            // to catch error if Endpoint Methode not exsist and if Method Not Allowed
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            // to worke Authentication JWT Service
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages(); // to serve Razor Pages
                endpoints.MapControllers();
            });
        }
    }
}
