using Microsoft.AspNetCore.Identity;
using WebAPI.Configs;
using WebAPI.Entities;

namespace WebAPI.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task SeedDefaultUser(
            this UserManager<User> userManager,
            DefaultUser defaultUser)
        {
            if(!string.IsNullOrWhiteSpace(defaultUser.Email) && !string.IsNullOrWhiteSpace(defaultUser.Password)) 
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = defaultUser.Email,
                        EmailConfirmed = true,
                        UserName = defaultUser.Email
                    };
                    var result = await userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        await userManager.AddPasswordAsync(user, defaultUser.Password);
                    }
                }
            }            
        }
    }
}
