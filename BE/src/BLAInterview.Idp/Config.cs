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

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "bla-interview-api" },
                Claims =
                [
                    new ClientClaim(type: "sub", value: "bla-interview-api-client")
                ]
            }
        ];
}