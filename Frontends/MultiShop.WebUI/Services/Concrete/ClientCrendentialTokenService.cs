using Duende.IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Settings;

namespace MultiShop.WebUI.Services.Concrete;

public class ClientCrendentialTokenService : IClientCrendentialTokenService
{
    private readonly ServiceAPISettings _serviceAPISettings;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ClientSettings _clientSettings;

    public ClientCrendentialTokenService(IOptions<ServiceAPISettings> serviceAPISettings, HttpClient httpClient, IMemoryCache memoryCache, IOptions<ClientSettings> clientSettings)
    {
        _serviceAPISettings = serviceAPISettings.Value;
        _httpClient = httpClient;
        _memoryCache = memoryCache;
        _clientSettings = clientSettings.Value;
    }

    public async Task<string> GetToken()
    {
        var currentToken = _memoryCache.Get<string>("multishoptoken");
        if (currentToken != null)
        {
            return currentToken;
        }

        var discoveryEndpoint = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceAPISettings.IdentityServerUrl,
            Policy = new DiscoveryPolicy
            {
                RequireHttps = true
            }
        });

        if (discoveryEndpoint.IsError)
        {
            throw new Exception($"Discovery document error: {discoveryEndpoint.Error}");
        }

        var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
        {
            ClientId = _clientSettings.MultiShopVisitorClient.ClientId,
            ClientSecret = _clientSettings.MultiShopVisitorClient.ClientSecret,
            Address = discoveryEndpoint.TokenEndpoint,
            // Catalog + Comment + Basket + Ocelot scopes
            Scope = "CatalogFullPermission CommentFullPermission BasketFullPermission OcelotFullPermission"
        };

        var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
        if (newToken.IsError || string.IsNullOrWhiteSpace(newToken.AccessToken))
        {
            throw new Exception($"Token request error: {newToken.Error} {newToken.ErrorDescription}");
        }

        var safeExpiresIn = newToken.ExpiresIn > 60 ? newToken.ExpiresIn - 30 : 60; // en az 60 sn cache'le
        _memoryCache.Set("multishoptoken", newToken.AccessToken, TimeSpan.FromSeconds(safeExpiresIn));
        return newToken.AccessToken;
    }
}
