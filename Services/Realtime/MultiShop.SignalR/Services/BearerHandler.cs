using System.Net.Http.Headers;
using Duende.IdentityModel.Client;

namespace MultiShop.SignalR.Services;

public sealed class BearerTokenCache
{
    private string? _token;
    private DateTime _expiresAt = DateTime.MinValue;
    private readonly object _lock = new();

    public bool TryGet(out string token)
    {
        lock (_lock)
        {
            if (!string.IsNullOrEmpty(_token) && DateTime.UtcNow < _expiresAt)
            {
                token = _token!;
                return true;
            }
            token = string.Empty;
            return false;
        }
    }

    public void Set(string token, int expiresInSeconds)
    {
        lock (_lock)
        {
            _token = token;
            _expiresAt = DateTime.UtcNow.AddSeconds(Math.Max(60, expiresInSeconds - 30));
        }
    }
}

public sealed class BearerHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;
    private readonly BearerTokenCache _cache;

    public BearerHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        _cache = new BearerTokenCache();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_cache.TryGet(out var token))
        {
            var authority = _configuration["Identity:Authority"] ?? "https://localhost:5001";
            var clientId = _configuration["Identity:ClientId"] ?? "MultiShopVisitorId";
            var clientSecret = _configuration["Identity:ClientSecret"] ?? "multishopvisitorsecret";
            var scope = _configuration["Identity:Scope"] ?? "CommentFullPermission MessageFullPermission OcelotFullPermission";

            using var http = new HttpClient();
            var disco = await http.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,
                Policy = new DiscoveryPolicy { RequireHttps = true }
            }, cancellationToken);
            if (disco.IsError || string.IsNullOrEmpty(disco.TokenEndpoint))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var tokenResponse = await http.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = scope
            }, cancellationToken);

            if (!tokenResponse.IsError && !string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                _cache.Set(tokenResponse.AccessToken!, tokenResponse.ExpiresIn);
                token = tokenResponse.AccessToken!;
            }
        }

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}


