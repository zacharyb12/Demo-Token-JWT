using BLL.Services.AuthServices;
using BLL.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Auth_Models;

namespace Demo_Token_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterForm form)
        {
            string? token = await _authService.RegisterAsync(form);
            
            if (token == null)
            {
                return BadRequest(new { message = "L'utilisateur existe déjà (username ou email)." });
            }

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Loginform form)
        {
            var result = await _authService.LoginAsync(form);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

                return Ok(result);
            
        }
    }
}
