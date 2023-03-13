using AutoMapper;
using Database;
using Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SmartWeight.Panel.Server.Controllers
{
    /// <summary>
    /// ���������� ��� ����������� / �������������� �������
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly FactoryManager _factoryManager;

        private readonly IMapper _mapper;
        public AuthorizationController(IMapper mapper
            , FactoryManager factoryManager
            , SignInManager<User> signInManager
            , UserManager<User> userManager
            , RoleManager<Role> roleManager)
        {
            _mapper = mapper;

            _factoryManager = factoryManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// ��������� ���� � �������
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignIn")]
        public async Task<OperationResult> SignInAsync(LoginRequest request)
        {
            if (await _userManager.FindByNameAsync(request.Login) is User user)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

                if (result.Succeeded)
                {
                    await _signInManager.SignInWithClaimsAsync(user, true, 
                        await _userManager.GetClaimsAsync(user));

                    SignIn(User, CookieAuthenticationDefaults.AuthenticationScheme);

                    return OperationResult.Ok();
                }
                else
                { 
                    _ = _userManager.AccessFailedAsync(user);
                }
            }

            return OperationResult.Failed("�� ������ ����� ��� ������");
        }
        /// <summary>
        /// ����� �� ��������
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("SignOut")]
        public Task SignOutAsync() => _signInManager.SignOutAsync();
        /// <summary>
        /// �������� ��������� �����������
        /// </summary>
        /// <returns></returns>
        [HttpGet("AuthorizationState")]
        public bool AuthorizationState() => _signInManager.IsSignedIn(User);
        /// <summary>
        /// �������� ��������� ��������������
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAuthenticationState")]
        public AuthenticationData GetAuthenticationState()
            => new AuthenticationData(new IdentityData(User));
    }
}