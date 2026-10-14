using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace VShop.IdentityServer.Configuration
{
    public class IdentityConfiguration
    {
        public const string Admin = "Admin";
        public const string Client = "Cliente";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("vshop", "VShop Server"),
                new ApiScope(name: "read", "Read data."),
                new ApiScope(name: "write", "Write data."),
                new ApiScope(name: "delete", "Delete data."),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("vshop", "VShop API")
                {
                    Scopes = { "vshop", "read", "write", "delete" },
                    UserClaims = { "role", "name", "email" }
                }
            };

        public static IEnumerable<Client> Clients =>
   new List<Client>
   {
               //cliente genérico
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("abracadabra#simsalabim".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials, //precisa das credenciais do usuário
                    AllowedScopes = {"read", "write", "profile" }
                },
                new Client
                {
                    ClientId = "vshop",
                    ClientSecrets = { new Secret("abracadabra#simsalabim".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code, //via codigo
                    RedirectUris = {"https://localhost:7118/signin-oidc"},//login
                    PostLogoutRedirectUris = {"https://localhost:7118/signout-callback-oidc"},//logout
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "vshop"
                    }
                },
                //Swagger UI do ProductApi
                new Client
                {
                    ClientId = "vshop-swagger",
                    ClientName = "VShop Swagger UI",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false, //client publico: usa PKCE no lugar do secret
                    RequirePkce = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        "https://localhost:7231/swagger/oauth2-redirect.html",
                        "http://localhost:5280/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:7231/swagger/index.html",
                        "http://localhost:5280/swagger/index.html"
                    },
                    AllowedCorsOrigins =
                    {
                        "https://localhost:7231",
                        "http://localhost:5280"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "vshop",
                        "read",
                        "write",
                        "delete"
                    }
                }
   };
    }
}
