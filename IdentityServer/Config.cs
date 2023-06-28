using Duende.IdentityServer.Models;
using IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources(IConfiguration configuration)
    {
        List<IdentityResource> identityResources = new()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
            new IdentityResources.Address()
        };

        string resourceName = configuration.GetSection("TestData:IdentityResource:Name").Value;
        string resourceDisplayName = configuration.GetSection("TestData:IdentityResource:DisplayName").Value;
        string userClaims = configuration.GetSection("TestData:IdentityResource:UserClaims").Value;

        List<string> additionalClaims = userClaims.Split(',').Select(claim => claim.Trim()).ToList();

        IdentityResource customIdentityResource = new(resourceName, resourceDisplayName, additionalClaims);
        identityResources.Add(customIdentityResource);

        return identityResources;
    }

    public static IEnumerable<ApiScope> ApiScopes(IConfiguration configuration)
    {
        string apiScopes = configuration.GetSection("TestData:ApiScope").Value;
        IEnumerable<string> scopes = apiScopes.Split(',').Select(scope => scope.Trim());
        IEnumerable<ApiScope> apiScopeList = scopes.Select(scope => new ApiScope(scope, scope));
        return apiScopeList;
    }

    public static IEnumerable<Client> Clients(IConfiguration configuration)
    {
        string clientId = configuration.GetSection("TestData:Client:ClientId").Value;
        string clientName = configuration.GetSection("TestData:Client:Name").Value;
        string clientSecret = configuration.GetSection("TestData:Client:ClientSecret").Value;
        string[] redirectUris = configuration.GetSection("TestData:Client:RedirectUris").Value.Split(',').Select(s => s.Trim()).ToArray();
        string[] postLogoutRedirectUris = configuration.GetSection("TestData:Client:PostLogoutRedirectUris").Value.Split(',').Select(s => s.Trim()).ToArray();
        string[] allowedScopes = configuration.GetSection("TestData:Client:AllowedScopes").Value.Split(',').Select(s => s.Trim()).ToArray();

        Client client = new()
        {
            ClientId = clientId,
            ClientName = clientName,
            ClientSecrets = { new Secret(clientSecret.Sha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = false,
            AllowRememberConsent = true,
            RedirectUris = redirectUris,
            PostLogoutRedirectUris = postLogoutRedirectUris,
            AllowedScopes = new List<string>(allowedScopes)
        };

        return new[] { client };
    }

    public static IEnumerable<IDictionary<ApplicationUser, string>> AdminUsers(IConfiguration configuration)
    {
        List<IDictionary<ApplicationUser, string>> adminUsers = new();

        ApplicationUser adminUser = new()
        {
            UserName = configuration.GetSection("TestData:AdminUser:UserName").Value,
            Email = configuration.GetSection("TestData:AdminUser:Email").Value,
            EmailConfirmed = configuration.GetSection("TestData:AdminUser:EmailConfirmed").Get<bool>(),
            PhoneNumber = configuration.GetSection("TestData:AdminUser:PhoneNumber").Value,
            FirstName = configuration.GetSection("TestData:AdminUser:FirstName").Value,
            LastName = configuration.GetSection("TestData:AdminUser:LastName").Value
        };
        
        Dictionary<ApplicationUser, string> adminUserDictionary = new()
        {
            { adminUser, configuration.GetSection("TestData:AdminUser:Password").Value }
        };

        adminUsers.Add(adminUserDictionary);

        return adminUsers;
    }

    public static IEnumerable<IDictionary<ApplicationUser, string>> RegularUsers(IConfiguration configuration)
    {
        List<IDictionary<ApplicationUser, string>> regularUsers = new();

        ApplicationUser regularUser = new()
        {
            UserName = configuration.GetSection("TestData:RegularUser:UserName").Value,
            Email = configuration.GetSection("TestData:RegularUser:Email").Value,
            EmailConfirmed = configuration.GetSection("TestData:RegularUser:EmailConfirmed").Get<bool>(),
            PhoneNumber = configuration.GetSection("TestData:RegularUser:PhoneNumber").Value,
            FirstName = configuration.GetSection("TestData:RegularUser:FirstName").Value,
            LastName = configuration.GetSection("TestData:RegularUser:LastName").Value
        };

        Dictionary<ApplicationUser, string> regularUserDictionary = new()
        {
            { regularUser, configuration.GetSection("TestData:RegularUser:Password").Value }
        };

        regularUsers.Add(regularUserDictionary);

        return regularUsers;
    }
}
