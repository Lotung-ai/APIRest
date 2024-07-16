using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Domain;
using System;
using System.Threading.Tasks;

namespace P7CreateRestApi.Services
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RoleSeeder> _logger;

        public RoleSeeder(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager, ILogger<RoleSeeder> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedRolesAsync()
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation("Role {RoleName} created", roleName);
                    }
                    else
                    {
                        _logger.LogError("Error creating role {RoleName}: {Errors}", roleName, roleResult.Errors);
                    }
                }
            }

         /*   // Créer un utilisateur administrateur par défaut
            var adminUser = new User
            {
                UserName = "Adminbdk",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            string adminPassword = "Brad@123456";

            var user = await _userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var createPowerUser = await _userManager.CreateAsync(adminUser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Admin user created and assigned to Admin role");
                }
                else
                {
                    _logger.LogError("Error creating admin user: {Errors}", createPowerUser.Errors);
                }
            }*/
        }
    }
}
