using Duende.IdentityServer.Models;

namespace BLAInterview.Idp.Config;
public static class Config
{
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
            }
        ];
}