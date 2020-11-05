using Adsbility.Appilication.Common.Models;
using Adsbility.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adsbility.Infrastructure.presistence
{
    public static class AppilicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager , RoleManager<ApplicationRole> roleManager)
        {
            var defaultUser = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" , EmailConfirmed = true};

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, "Administrator1!");
                if (!await roleManager.RoleExistsAsync(AuthurizationRoles.Admin))
                    await SeedRoles(roleManager);
                await userManager.AddToRoleAsync(defaultUser, AuthurizationRoles.Admin);
            }
        }

        public static async Task SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (roleManager.Roles.Count() < 1)
            {
                var role = new ApplicationRole
                {
                    Name = AuthurizationRoles.Admin
                };
                await roleManager.CreateAsync(role);
                role = new ApplicationRole
                {
                    Name = AuthurizationRoles.Company
                };
                await roleManager.CreateAsync(role);
                role = new ApplicationRole
                {
                    Name = AuthurizationRoles.Agent
                };
                await roleManager.CreateAsync(role);
                role = new ApplicationRole
                {
                    Name = AuthurizationRoles.SP
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
