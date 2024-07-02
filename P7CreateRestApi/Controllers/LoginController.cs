using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;
using P7CreateRestApi.Domain;
using System.Threading.Tasks;
using P7CreateRestApi.Services;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;

        public LoginController(SignInManager<User> signInManager, TokenService tokenService)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        // POST api/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid login attempt.", Errors = ModelState });
            }

            // Attempt to sign in the user
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(model.UserName);
                var token = await _tokenService.GenerateTokenAsync(user);
                return Ok(new { Message = "Login successful.", Token=token });

            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new { Message = "User account is locked out." });
            }

            if (result.IsNotAllowed)
            {
                return Unauthorized(new { Message = "User is not allowed to log in." });
            }

            return Unauthorized(new { Message = "Invalid login attempt." });
        }

        // POST api/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful." });
        }
    }
}
