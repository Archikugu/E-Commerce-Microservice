using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace MultiShop.IdentityServer;

public static class Config
{
    public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
    {
        new ApiResource("ResourceCatalog")
        {
            Scopes = { "CatalogFullPermission", "CatalogReadPermission" }
        },
        new ApiResource("ResourceDiscount")
        {
            Scopes = { "DiscountFullPermission" }
        },
        new ApiResource("ResourceOrder")
        {
            Scopes = { "OrderFullPermission" }
        },
        new ApiResource("ResourceCargo")
        {
            Scopes = { "CargoFullPermission" }
        },
        new ApiResource("ResourceBasket")
        {
            Scopes = { "BasketFullPermission" }
        },
        new ApiResource("ResourceComment")
        {
            Scopes = { "CommentFullPermission" }
        },
        new ApiResource("ResourceImages")
        {
            Scopes = { "ImagesFullPermission" }
        },
        new ApiResource("ResourcePayment")
        {
            Scopes = { "PaymentFullPermission" }
        },
        new ApiResource("ResourceMessage")
        {
            Scopes = { "MessageFullPermission" }
        },
        new ApiResource("ResourceOcelot")
        {
            Scopes = { "OcelotFullPermission" }
        },
        new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
    };

    public static IEnumerable<IdentityResource> IdentityResources = new IdentityResource[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Email(),
        new IdentityResources.Profile()
    };

    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
        new ApiScope("CatalogFullPermission", "Full access to catalog operations"),
        new ApiScope("CatalogReadPermission", "Read-only access to catalog operations"),
        new ApiScope("DiscountFullPermission", "Full access to discount operations"),
        new ApiScope("OrderFullPermission", "Full access to order operations"),
        new ApiScope("CargoFullPermission", "Full access to cargo operations"),
        new ApiScope("BasketFullPermission", "Full access to basket operations"),
        new ApiScope("CommentFullPermission", "Full access to comment operations"),
        new ApiScope("ImagesFullPermission", "Full access to images operations"),
        new ApiScope("PaymentFullPermission", "Full access to payment operations"),
        new ApiScope("MessageFullPermission", "Full access to message operations"),
        new ApiScope("OcelotFullPermission", "Full access to all services through Ocelot Gateway"),
        new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
    };

    public static IEnumerable<Client> Clients => new Client[]
    {
        // Visitor
        new Client
        {
            ClientId = "MultiShopVisitorId",
            ClientName = "Multi Shop Visitor User",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("multishopvisitorsecret".Sha256()) },
            AllowedScopes = { "CatalogReadPermission", "DiscountFullPermission","CommentFullPermission", "ImagesFullPermission","CatalogFullPermission", "OcelotFullPermission","BasketFullPermission",
                IdentityServerConstants.LocalApi.ScopeName, }
        },

        // Manager
        new Client
        {
            ClientId = "MultiShopManagerId",
            ClientName = "Multi Shop Manager User",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("multishopmanagersecret".Sha256()) },
            AllowedScopes = { "CatalogFullPermission", "BasketFullPermission", 
                "DiscountFullPermission", "OrderFullPermission", "MessageFullPermission",
                "CommentFullPermission","ImagesFullPermission",
                "PaymentFullPermission","OcelotFullPermission",
            IdentityServerConstants.LocalApi.ScopeName,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.OfflineAccess },
            AllowOfflineAccess = true
        },

        // Admin
        new Client
        {
            ClientId = "MultiShopAdminId",
            ClientName = "Multi Shop Admin User",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("multishopadminsecret".Sha256()) },
            AllowedScopes =
            {
                "CatalogFullPermission",
                "DiscountFullPermission",
                "OrderFullPermission",
                "CargoFullPermission",
                "BasketFullPermission",
                "CommentFullPermission",
                "ImagesFullPermission",
                "PaymentFullPermission",
                "MessageFullPermission",
                "OcelotFullPermission",
                IdentityServerConstants.LocalApi.ScopeName,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.OfflineAccess
            },
            AllowOfflineAccess = true,
            AccessTokenLifetime = 600
        }
    };
}
