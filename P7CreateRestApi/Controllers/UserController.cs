using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager; 
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public UserController(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifier si le rôle existe, sinon le créer
            if (!await _roleManager.RoleExistsAsync(register.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
            }

            var user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
                Fullname = register.Fullname,
                Role = register.Role
            };

            // Créer l'utilisateur avec un mot de passe
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, register.Role);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        // Endpoint pour mettre à jour un utilisateur
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterModel register)
        {
            if (id <= 0 || register == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = register.UserName;
            user.Email = register.Email;
            user.Fullname = register.Fullname;
            user.Role = register.Role;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(register.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int> { Name = register.Role });
                }
                await _userManager.AddToRoleAsync(user, register.Role); // Assurer que le rôle est mis à jour
                return Ok(user);
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Vérifiez si l'utilisateur existe avant la suppression
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            var result = await _userRepository.DeleteUserAsync(id);
            if (!result)
            {
                return BadRequest("Failed to delete user.");
            }

            return NoContent();
        }
    }
}
