using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace BLAInterview.Idp.Config;

public static class Config
{
    public const string SpaClientId = "bla-interview-spa";

    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            // Duende requires at least one user claim on IdentityResource; offline_access does not
            // define profile claims — sub satisfies the ctor and is always issued with openid.
            new IdentityResource(
                IdentityServerConstants.StandardScopes.OfflineAccess,
                "Offline access",
                userClaims: [JwtClaimTypes.Subject])
        ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource(name: "bla-interview-api", displayName: "BLA Interview API")
            {
                Scopes = { "bla-interview-api" }
            }
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope(name: "bla-interview-api", displayName: "BLA Interview API")
        ];

    public static IEnumerable<Client> Clients =>
        [
            new Client
            {
                ClientId = "bla-interview-api-client",

                // The simulator client authenticates a registered user without a browser UI.
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "bla-interview-api" }
            },
            new Client
            {
                ClientId = SpaClientId,
                ClientName = "BLA Interview SPA",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                AllowOfflineAccess = true,
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                RedirectUris =
                {
                    "http://localhost:5173/auth/callback",
                    "https://localhost:5173/auth/callback",
                    "http://127.0.0.1:5173/auth/callback",
                    "https://127.0.0.1:5173/auth/callback"
                },
                PostLogoutRedirectUris =
                {
                    "http://localhost:5173/",
                    "http://localhost:5173/login",
                    "https://localhost:5173/",
                    "https://localhost:5173/login",
                    "http://127.0.0.1:5173/",
                    "http://127.0.0.1:5173/login",
                    "https://127.0.0.1:5173/",
                    "https://127.0.0.1:5173/login"
                },
                AllowedCorsOrigins =
                {
                    "http://localhost:5173",
                    "https://localhost:5173",
                    "http://127.0.0.1:5173",
                    "https://127.0.0.1:5173"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "bla-interview-api"
                }
            }
        ];
}
