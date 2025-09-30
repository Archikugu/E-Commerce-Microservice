using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Handlers;

public class ResourceOwnerPasswordTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;

    public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, IIdentityService identityService)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectParameterNames.AccessToken);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            // Fallback to claim
            accessToken = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "access_token")?.Value;
        }
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            // Fallback to auth ticket store
            var authResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var tokenFromStore = authResult?.Properties?.GetTokenValue(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrWhiteSpace(tokenFromStore))
            {
                accessToken = tokenFromStore;
            }
        }
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await _identityService.GetRefreshToken();
            if (refreshed)
            {
                var newAccessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectParameterNames.AccessToken);
                if (string.IsNullOrWhiteSpace(newAccessToken))
                {
                    newAccessToken = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "access_token")?.Value;
                }
                if (!string.IsNullOrWhiteSpace(newAccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            //Error Message
        }

        return response;
    }
}
