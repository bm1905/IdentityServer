using System.Security.Claims;
using IdentityModel;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace IdentityServer.Initializer;

public class DbInitializer
{
    public const string Admin = "admin";
    public const string Regular = "regular";

    public static async Task<bool> EnsureSeedData(WebApplication app)
    {
        using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetService<ApplicationDbContext>();

        if (context == null) throw new Exception("Null ApplicationDbContext");

        // Creates database and runs migration if database does not exist.
        await context.Database.EnsureCreatedAsync();

        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await roleManager.FindByNameAsync(Admin) == null)
        {
            await roleManager.CreateAsync(new IdentityRole(Admin));
            ApplicationUser adminUser = new ApplicationUser
            {
                UserName = "admin_user",
                Email = "bijay@admin.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                FirstName = "Bijay",
                LastName = "Maharjan"
            };
            await CreateUser(userManager, adminUser, "Test123*", Admin);

            if (await roleManager.FindByNameAsync(Regular) != null) return true;

            await roleManager.CreateAsync(new IdentityRole(Regular));
            ApplicationUser regularUser = new ApplicationUser
            {
                UserName = "regular_user",
                Email = "bijay@regular.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                FirstName = "Bijay",
                LastName = "Maharjan"
            };
            await CreateUser(userManager, regularUser, "Test123*", Regular);
        }
        else
        {
            Log.Debug($"{Admin} and {Regular} roles already exist");
            return true;
        }

        return true;
    }

    private static async Task CreateUser(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string roleName)
    {
        IdentityResult createUserResult = await userManager.CreateAsync(user, password);
        if (!createUserResult.Succeeded)
        {
            throw new Exception(createUserResult.Errors.First().Description);
        }

        IdentityResult addToRoleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!addToRoleResult.Succeeded)
        {
            throw new Exception(addToRoleResult.Errors.First().Description);
        }

        IdentityResult addClaimsResult = await userManager.AddClaimsAsync(user, new Claim[]{
            new(JwtClaimTypes.Name, user.FirstName + " " + user.LastName),
            new(JwtClaimTypes.GivenName, user.FirstName),
            new(JwtClaimTypes.FamilyName, user.LastName),
            new(JwtClaimTypes.Role, roleName)
        });

        if (!addClaimsResult.Succeeded)
        {
            throw new Exception(addClaimsResult.Errors.First().Description);
        }
        Log.Debug($"User with {roleName} role created");
    }
}
