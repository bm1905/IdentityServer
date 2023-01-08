using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new("roles", "Your role(s)", new List<string>() { "role" })
        };

    public static IEnumerable<ApiScope> ApiScopes(IConfiguration configuration) =>
        new ApiScope[]
        {
            new(configuration.GetSection("ApiScopes:FinanceServices").Value, "FinanceServices"),
            new(configuration.GetSection("ApiScopes:TaxServices").Value, "TaxServices"),
            new(configuration.GetSection("ApiScopes:WageServices").Value, "WageServices")
        };

    public static IEnumerable<Client> Clients(IConfiguration configuration) =>
        new Client[]
        {
            new()
            {
                ClientId = configuration.GetSection("Clients:WebClient:ClientId").Value,
                ClientName = configuration.GetSection("Clients:WebClient:ClientName").Value,
                ClientSecrets = { new Secret(configuration.GetSection("Clients:WebClient:ClientSecret").Value.Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = false,
                AllowRememberConsent = true,
                RedirectUris = { $"{configuration.GetSection("Clients:WebClient:Uri").Value}/signin-oidc" },
                PostLogoutRedirectUris = { $"{configuration.GetSection("Clients:WebClient:Uri").Value}/signout-callback-oidc" },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    configuration.GetSection("ApiScopes:WageServices").Value,
                    configuration.GetSection("ApiScopes:FinanceServices").Value,
                    configuration.GetSection("ApiScopes:TaxServices").Value
                }
            }
        };
}
