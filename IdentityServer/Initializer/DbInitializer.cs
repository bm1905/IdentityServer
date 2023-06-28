using System.Security.Claims;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IdentityModel;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Models.Exceptions;

namespace IdentityServer.Initializer;

public class DbInitializer
{
    public const string Admin = "admin";
    public const string Regular = "regular";

    public static async Task<bool> EnsureSeedData(WebApplication app)
    {
        using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        IConfiguration configuration = app.Services.GetRequiredService<IConfiguration>();

        ApplicationDbContext applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        ConfigurationDbContext configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        PersistedGrantDbContext persistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();

        if (applicationDbContext is null || configurationDbContext is null || persistedGrantDbContext is null) throw new InternalServerErrorException("Null ApplicationDbContext");

        // Creates database and runs migration if database does not exist.
        await applicationDbContext.Database.EnsureCreatedAsync();
        await configurationDbContext.Database.EnsureCreatedAsync();
        await persistedGrantDbContext.Database.EnsureCreatedAsync();

        // Add dummy data
        if (!await configurationDbContext.Clients.AnyAsync())
        {
            foreach (Client client in Config.Clients(configuration))
            {
                configurationDbContext.Clients.Add(client.ToEntity());
            }

            await configurationDbContext.SaveChangesAsync();
        }

        if (!await configurationDbContext.IdentityResources.AnyAsync())
        {
            foreach (IdentityResource resource in Config.IdentityResources(configuration))
            {
                configurationDbContext.IdentityResources.Add(resource.ToEntity());
            }
            await configurationDbContext.SaveChangesAsync();
        }

        if (!await configurationDbContext.ApiScopes.AnyAsync())
        {
            foreach (ApiScope resource in Config.ApiScopes(configuration))
            {
                configurationDbContext.ApiScopes.Add(resource.ToEntity());
            }
            await configurationDbContext.SaveChangesAsync();
        }

        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await roleManager.FindByNameAsync(Admin) == null)
        {
            await roleManager.CreateAsync(new IdentityRole(Admin));
            IEnumerable<IDictionary<ApplicationUser, string>> adminUsers = Config.AdminUsers(configuration);

            foreach (IDictionary<ApplicationUser, string> adminUserEntry in adminUsers)
            {
                ApplicationUser adminUser = adminUserEntry.Keys.First();
                string adminUserPassword = adminUserEntry.Values.First();
                await CreateUser(userManager, adminUser, adminUserPassword, Admin);
            }

            if (await roleManager.FindByNameAsync(Regular) != null) return true;

            await roleManager.CreateAsync(new IdentityRole(Regular));
            IEnumerable<IDictionary<ApplicationUser, string>> regularUsers = Config.RegularUsers(configuration);

            foreach (IDictionary<ApplicationUser, string> regularUserEntry in regularUsers)
            {
                ApplicationUser regularUser = regularUserEntry.Keys.First();
                string regularUserPassword = regularUserEntry.Values.First();
                await CreateUser(userManager, regularUser, regularUserPassword, Regular);
            }
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
