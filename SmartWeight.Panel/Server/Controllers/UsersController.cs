using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SmartWeight.Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public UserController(
            IMapper mapper,
            RoleManager<Role> roleManager,
            UserManager<User> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateUser")]
        public async Task<OperationResult> CreateUserAsync(
            CreateFactoryUserRequest request)
        { 
            var user = _mapper.Map<User>(request);
            var factories = _mapper.Map<IEnumerable<Factory>>(request.Factories);
            var password = request.Password;
            var result = await _userManager.CreateAsync(user, password);
            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            return OperationResult.FromIdentityResult(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUser")]
        public Task<IdentityResult> UpdateUserAsync(
            User user)
            => _userManager.UpdateAsync(user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser")]
        public Task<IdentityResult> DeleteUserAsync(User user)
            => _userManager.DeleteAsync(user);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public IQueryable<User> GetUsers()
            => _userManager.Users;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrentUser")]
        public Task<User?> GetCurrentUser()
            => _userManager.GetUserAsync(User);

        [HttpGet("Search")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<User> SearchAsync(string? query = "")
        {
            var users = _userManager.Users.AsEnumerable();

            return users
                .Where(user => user.Email?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                               user.PhoneNumber?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                               user.UserName?.Contains(query, StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}
