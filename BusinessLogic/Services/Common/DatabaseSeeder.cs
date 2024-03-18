using BusinessLogic.Constants.Role;
using BusinessLogic.Constants.User;
using BusinessLogic.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Services.Common
{
    public interface IDatabaseSeeder
    {
        void Initialize();
    }

    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public DatabaseSeeder(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ILogger<DatabaseSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public void Initialize()
        {
            AddAdministrator();
            AddAdminUser();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var sysAdminRole = new AppRole(RoleConstants.Admin, "Administrator role with full permissions");
                var sysAdminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Admin);
                if (sysAdminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(sysAdminRole);
                    sysAdminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Admin);
                    _logger.LogInformation("Seeded Administrator Role.");
                }
                //Check if User Exists
                var superUser = new AppUser
                {
                    FullName = "NinePlus Solution",
                    Email = "superadmin@gmail.com",
                    UserName = "superadmin",
                    PhoneNumber = "0235548632",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true, 
                    Password = UserConstants.DefaultPassword
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.Admin);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Seeded Default SuperAdmin User.");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }
            }).GetAwaiter().GetResult();
        }

        private void AddAdminUser()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var adminRole = new AppRole(RoleConstants.Admin, "User role with default permissions");
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Admin);
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    _logger.LogInformation("Seeded Admin Role.");
                }
                ////Check if User Exists
                //var adminUser = new AppUser()
                //{
                //    FullName = "Admin User",
                //    Email = "admin.user@gmail.com",
                //    UserName = "admin",
                //    EmailConfirmed = true,
                //    PhoneNumberConfirmed = true,
                //    CreatedOn = DateTime.Now,
                //    IsActive = true
                //};
                //var adminUserInDb = await _userManager.FindByNameAsync(adminUser.UserName);
                //if (adminUserInDb == null)
                //{
                //    await _userManager.CreateAsync(adminUser, UserConstants.DefaultPassword);
                //    await _userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
                //    _logger.LogInformation("Seeded User with Admin Role.");
                //}
            }).GetAwaiter().GetResult();
        }
    }
}