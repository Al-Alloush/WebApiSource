using Core.Entities.AppSettings;
using Core.Entities.Identity;
using Core.EntitiesDTOs.AppSettings;
using Infrastructure.DataApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiBackend.Controllers.AppSettings
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]/*to not display https://localhost:5001/swagger/ as an error in api swagger*/
    public class InitializeAppController : ControllerBase
    {
        private readonly RoleManager<AppIdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public InitializeAppController(RoleManager<AppIdentityRole> roleManager,
                                        UserManager<AppUser> userManager,
                                        IConfiguration config,
                                        AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult<string>> InitializeApp(InitializeAppDto initializeApp )
        {

            // Add AppRoles in Role Table
            await AddAppRoles();

            // Add AppLanguage
            await AddAppLanguage();


            // Create AuperAdmin Account
            await AddSuperAdminUser(initializeApp.Email, initializeApp.UserName, initializeApp.Password);

            return "Initialize Website Successfully";
        }

        [HttpPost("seedUsers")]
        public async Task<bool> SeedUsers()
        {

            List<AppUser> usersDbList = await _context.Users.ToListAsync();

            // this right path becaulse this class will running from Program.cs inside API project.
            string usersJsonString = System.IO.File.ReadAllText(_config["InfrastructureDataAppSeedData"] + "SeedUsers.json");

            List<AppUser> usersJsonList = JsonSerializer.Deserialize<List<AppUser>>(usersJsonString);
   
            foreach (var user in usersJsonList)
            {
                if (!usersDbList.Any(x=>x.Email == user.Email))
                {
                    IdentityResult create = await _userManager.CreateAsync(user, "!QA1qa");
                    if (create.Succeeded)
                    {
                        if (user.UserName.ToUpper().Trim().Contains("ADMIN"))
                            await _userManager.AddToRoleAsync(user, "Admin");
                        else if (user.UserName.ToUpper().Trim().Contains("EDITOR"))
                            await _userManager.AddToRoleAsync(user, "Editor");
                        else
                            await _userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            return true;
        }

        private async Task<bool> AddAppRoles()
        {
            try
            {
                // get all Roles from Roles Table
                List<AppIdentityRole> rolesDbList = await _roleManager.Roles.ToListAsync();

                // this right path becaulse this class will running from Program.cs inside API project.
                string rolesJsonString = System.IO.File.ReadAllText(_config["InfrastructureDataAppSeedData"] + "AppRoles.json");

                List<AppIdentityRole> rolesJsonList = JsonSerializer.Deserialize<List<AppIdentityRole>>(rolesJsonString);

                for (int i = 0; i < rolesJsonList.Count(); i++)
                {
                    bool exist = false;
                    for (int j = 0; j < rolesDbList.Count; j++)
                    {
                        if (rolesJsonList[i].Name == rolesDbList[j].Name)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                        await _roleManager.CreateAsync(rolesJsonList[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }


        private async Task<bool> AddAppLanguage()
        {
            try
            {
                // get all Languages in Language Table
                List<AppLanguage> langaugesDbList =  await _context.AppLanguages.ToListAsync();

                // get all Langauges from Json file as string
                string languageString = System.IO.File.ReadAllText(_config["InfrastructureDataAppSeedData"] + "AppLanguages.json");

                List<AppLanguage> languagesJsonList = JsonSerializer.Deserialize<List<AppLanguage>>(languageString);

                for (int i = 0; i < languagesJsonList.Count(); i++)
                {
                    bool exist = false;
                    for (int j = 0; j < langaugesDbList.Count; j++)
                    {
                        if (languagesJsonList[i].Name == langaugesDbList[j].Name)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                        await _context.AppLanguages.AddAsync(languagesJsonList[i]);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        private async Task<bool> AddSuperAdminUser(string email, string userName, string password)
        {
            // get the Accept-Langugae from Request Header
            string langId = HttpContext.Request.GetTypedHeaders().AcceptLanguage.FirstOrDefault()?.Value.Value;
            langId = langId != null ? langId.Substring(0, 2) : "en";

            // app has just one SuperAdmin, we don't want retrun it with any query
            List<AppUser> superAdmin = new List<AppUser>(await _userManager.GetUsersInRoleAsync("SuperAdmin"));
            if (superAdmin.Count >= 1)
                return true;

            CultureInfo MyCultureInfo = new CultureInfo("de-DE");

            // add new super admin
            var user = new AppUser
            {
                Email = email,
                EmailConfirmed = true,
                UserName = userName,
                LanguageId = langId,
                Birthday = DateTime.Parse("1/1/1970", MyCultureInfo)
            };

            IdentityResult create = await _userManager.CreateAsync(user, password);
            if (create.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "SuperAdmin");
            }

            return true;
        }


        
    }
}
