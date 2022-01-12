using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace IdentityServer;

public static class IdentityConfigurationProduction
{
    public static List<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };
    }

    public static List<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new("PrimeflixApi")
            {
                UserClaims = {"name", "email", "nameidentifier"},
                Scopes =
                {
                    "api.read"
                }
            }
        };
    }

    public static List<Client> GetClients()
    {
        return new List<Client>
        {
            new()
            {
                ClientId = "Primeflix",
                RequireClientSecret = false,
                AllowedCorsOrigins = new List<string> { "https://brave-bay-07a00b203.azurestaticapps.net" },
                Properties = new Dictionary<string, string>
                {
                    {ApplicationProfilesPropertyNames.Profile, ApplicationProfiles.SPA}
                },
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RedirectUris = {"https://brave-bay-07a00b203.azurestaticapps.net/authentication/login-callback"},
                PostLogoutRedirectUris = {"https://brave-bay-07a00b203.azurestaticapps.net/authentication/logout-callback"},
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api.read"
                },
                AllowAccessTokensViaBrowser = true,
                AccessTokenLifetime = 86400,
                RequireConsent = false,
                UpdateAccessTokenClaimsOnRefresh = true,
                AllowOfflineAccess = true
            }
        };
    }
}