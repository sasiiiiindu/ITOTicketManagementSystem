using ITOTicketManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace ITOTicketManagementSystem.Services
{
    public class DataSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedRolesAndUsersAsync()
        {
            // --- 1. SEED ROLES ---
            string[] roleNames = { "Admin", "Help Desk Team", "Engineering Team", "Employees" };

            foreach (var roleName in roleNames)
            {
                // Check if the role already exists
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the new role
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // --- 2. SEED USERS ---

            // Create Admin User
            await CreateUserAsync("admin@test.com", "Password123!", "Admin");

            // Create Help Desk Users
            await CreateUserAsync("helpdesk1@test.com", "Password123!", "Help Desk Team");
            await CreateUserAsync("helpdesk2@test.com", "Password123!", "Help Desk Team");

            // Create Engineering Team Users
            await CreateUserAsync("engineer1@test.com", "Password123!", "Engineering Team");
            await CreateUserAsync("engineer2@test.com", "Password123!", "Engineering Team");
            await CreateUserAsync("engineer3@test.com", "Password123!", "Engineering Team");

            // Create General Employee Users
            await CreateUserAsync("employee1@test.com", "Password123!", "Employees");
            await CreateUserAsync("employee2@test.com", "Password123!", "Employees");
            await CreateUserAsync("employee3@test.com", "Password123!", "Employees");
            await CreateUserAsync("employee4@test.com", "Password123!", "Employees");
        }

        private async Task CreateUserAsync(string email, string password, string role)
        {
            // Check if the user already exists
            if (await _userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true // Bypass email confirmation for dummy users
                };

                // Create the user with the specified password
                var result = await _userManager.CreateAsync(user, password);

                // Assign the user to the specified role
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}