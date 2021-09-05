using AutoMapper;
using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Infrastructure.DataApp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Identity
{
    public class AppRoleManager : RoleManager<AppIdentityRole>
    {
        private readonly IRoleStore<AppIdentityRole> _store;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AppRoleManager(
            IRoleStore<AppIdentityRole> store, 
            IEnumerable<IRoleValidator<AppIdentityRole>> roleValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            ILogger<RoleManager<AppIdentityRole>> logger,
            AppDbContext context,
            IMapper mapper)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _store = store;
            _context = context;
            _mapper = mapper;
        }


        public async  Task<IReadOnlyList<AppIdentityRole>> ReadRolesList()
        {
            List<AppIdentityRole> roles = await _context.Roles.Where(x => x.Name != "SuperAdmin").ToListAsync();

            return roles;
        }

        public async Task<string> CreateAsync(UserRoleDto userRoleDto)
        {
            // check if this role exist in Role table
            AppIdentityRole exsitRole = await FindByNameAsync(userRoleDto.Name.Trim());
            if (exsitRole != null)
                return "RoleNameExsitingBefore";

            // check if permaionLevel <= 10, no permation strong than Admin and SuperAdmin
            if (userRoleDto.PermissionLevel <= 10)
                return "PermissionLevelInvalied";

            // check if other Role hase the same PermationLevel
            var existPermationLevel = await _context.Roles.Where(x => x.PermissionLevel == userRoleDto.PermissionLevel).FirstOrDefaultAsync();
            if (existPermationLevel != null)
                return "PermissionLevelInvalied";


            var role = _mapper.Map<UserRoleDto, AppIdentityRole>(userRoleDto);

            var newRole = await CreateAsync(role);

            return "AddRoleSuccessfully";
        }

        public async Task<string> DeleteAsync(string role)
        {
            // check if this role exist in Role table
            AppIdentityRole exsitRole = await FindByNameAsync(role.Trim());
            if (exsitRole == null)
                return "RoleNameNotExsit";

            // check if this role has superAdmin permation
            if (exsitRole.PermissionLevel == 1 || 
                exsitRole.Name == "SuperAdmin" ||
                exsitRole.Name == "Admin" ||
                exsitRole.Name == "Editor" ||
                exsitRole.Name == "User")
                return "RoleNameNotExist";


            IdentityResult deleteRole = await DeleteAsync(exsitRole);
            if(deleteRole.Succeeded)
                return "DeleteRoleSuccessfully";
                
            return "DeleteRoleFailed";
        }
    }
}
