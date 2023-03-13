using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SmartWeight.Panel.Server.Controllers
{
    /// <summary>
    /// API контроллер для взаимодействия с ролями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private RoleManager<Role> _roleManager;
        public RolesController(RoleManager<Role> roleManager) 
        { 
            _roleManager= roleManager;
        }
        /// <summary>
        /// Создать роль
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost("CreateRole")]
        public Task<IdentityResult> CreateRoleAsync(string roleName)
            => _roleManager.CreateAsync(new Role() { Name = roleName });
        /// <summary>
        /// Удалить роль
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpDelete("DeleteRole")]
        public async Task<IdentityResult> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var result = await _roleManager.DeleteAsync(role);

            return result;
        }
        /// <summary>
        /// Получить роли
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllRoles")]
        public IQueryable<Role> GetRoles()
            => _roleManager.Roles;
    }
}
