using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SmartWeight.Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private RoleManager<Role> _roleManager;
        public RolesController(RoleManager<Role> roleManager) 
        { 
            _roleManager= roleManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateRole")]
        public Task<IdentityResult> CreateRoleAsync(string roleName)
            => _roleManager.CreateAsync(new Role() { Name = roleName });

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteRole")]
        public async Task<IdentityResult> DeleteRoleAsync(string roleName)
        {
            if (await _roleManager.FindByNameAsync(roleName) is Role role)
            { 
                return await _roleManager.DeleteAsync(role);
            }

            return IdentityResult.Failed();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllRoles")]
        public IQueryable<Role> GetRoles()
            => _roleManager.Roles;
    }
}
