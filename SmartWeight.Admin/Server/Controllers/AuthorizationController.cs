using Microsoft.AspNetCore.Mvc;

namespace SmartWeight.Admin.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        public AuthorizationController() { }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync()
        {

            return Ok();
        }

        public async Task<IActionResult> LogOutAsync() 
        {

            return Ok();
        }
    }
}