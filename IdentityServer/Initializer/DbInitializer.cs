using System.Security.Claims;
using IdentityModel;
using IdentityServer.DbContext;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            bool dbCreated = _db.Database.EnsureCreated();
            if (dbCreated)
            {
                // _db.Database.Migrate();
            }

            if (_roleManager.FindByNameAsync(Config.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(Config.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Config.Customer)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            // Test admin user
            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin_bijay",
                Email = "bijay@admin.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                FirstName = "Bijay",
                LastName = "Maharjan"
            };
            _userManager.CreateAsync(adminUser, "Test123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, Config.Admin).GetAwaiter().GetResult();
            IdentityResult tempAdmin = _userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new(JwtClaimTypes.Name, adminUser.FirstName + " " + adminUser.LastName),
                new(JwtClaimTypes.GivenName, adminUser.FirstName),
                new(JwtClaimTypes.FamilyName, adminUser.LastName),
                new(JwtClaimTypes.Role, Config.Admin)
            }).Result;

            // Test normal user
            ApplicationUser regularUser = new ApplicationUser()
            {
                UserName = "regular_bijay",
                Email = "bijay@regular.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                FirstName = "Bijay",
                LastName = "Maharjan"
            };
            _userManager.CreateAsync(regularUser, "Test123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(regularUser, Config.Customer).GetAwaiter().GetResult();
            IdentityResult tempRegular = _userManager.AddClaimsAsync(regularUser, new Claim[]
            {
                new(JwtClaimTypes.Name, regularUser.FirstName + " " + regularUser.LastName),
                new(JwtClaimTypes.GivenName, regularUser.FirstName),
                new(JwtClaimTypes.FamilyName, regularUser.LastName),
                new(JwtClaimTypes.Role, Config.Customer)
            }).Result;
        }
    }
}