using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBackend.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class UserRoleController : ControllerBase
    {
        private readonly AppRoleManager _roleManager;

        public UserRoleController(AppRoleManager roleManager )
        {
            _roleManager = roleManager;
        }


        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppIdentityRole>>> GetRoleList()
        {
            IReadOnlyList<AppIdentityRole> roles = await _roleManager.ReadRolesList();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateNewRole(UserRoleDto userRoleDto)
        {
            string result = await _roleManager.CreateAsync(userRoleDto);
            return Ok(result);
        }


        [HttpDelete("{role}")]
        public async Task<ActionResult<string>> DeleteRole(string role)
        {
            string result = await _roleManager.DeleteAsync(role);
            return Ok(result);
        }

    }
}
