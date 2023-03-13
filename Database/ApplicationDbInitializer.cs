using Constants.Server;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Database
{
    public static class ApplicationFolderInitializer
    {
        public static void SeedFolders()
        { 
            if (!Directory.Exists(Folders.Builds))
                Directory.CreateDirectory(Folders.Builds);
        }
    }
    public static class ApplicationDbInitializer
    {
        public static async Task SeedRoles(
            RoleManager<Role> roleManager,
            IList<string> roles)
        {
            if (roles is null)
            {
                throw new ArgumentNullException(nameof(roles));
            }

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role() 
                    { Name = role });
                }
            }
        }

        public static async Task SeedUsers(
            UserManager<User> userManager,
            string email, 
            string userName, 
            string password,
            IList<string> roles)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                User user = new User
                {
                    UserName = userName,
                    Email = email
                };

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}