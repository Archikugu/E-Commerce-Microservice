using System.Security.Claims;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MultiShop.WebUI.Dtos.IdentityDtos.LoginDtos;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Settings;

namespace MultiShop.WebUI.Services.Concrete;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClientSettings _clientSettings;
    private readonly ServiceAPISettings _serviceAPISettings;

    public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceAPISettings> serviceAPISettings)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _clientSettings = clientSettings.Value;
        _serviceAPISettings = serviceAPISettings.Value;
    }

    public async Task<bool> GetRefreshToken()
    {
        var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
        
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        var discoveryEndPoint = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceAPISettings.IdentityServerUrl,
            Policy = new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = false
            }
        });

        RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest
        {
            ClientId = _clientSettings.MultiShopManagerClient.ClientId,
            ClientSecret = _clientSettings.MultiShopManagerClient.ClientSecret,
            RefreshToken = refreshToken,
            Address = discoveryEndPoint.TokenEndpoint
        };

        try
        {
            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

            var authenticationToken = new List<AuthenticationToken>() {
                 new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = token.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = token.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.ExpiresIn,
                    Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o")
                }
            };
             
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var properties = result.Properties;
            properties.StoreTokens(authenticationToken);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Principal, properties);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SignIn(SignInDto signInDto)
    {
        var discoveryEndPoint = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceAPISettings.IdentityServerUrl,
            Policy = new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = false
            }
        });

        var passwordTokenRequest = new PasswordTokenRequest
        {
            ClientId = _clientSettings.MultiShopAdminClient.ClientId,
            ClientSecret = _clientSettings.MultiShopAdminClient.ClientSecret,
            UserName = signInDto.UserName,
            Password = signInDto.Password,
            Address = discoveryEndPoint.TokenEndpoint
        };

        var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
        var userInfo = await _httpClient.GetUserInfoAsync(new UserInfoRequest
        {
            Address = discoveryEndPoint.UserInfoEndpoint,
            Token = token.AccessToken
        });

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProps = new AuthenticationProperties();
        authProps.StoreTokens(new List<AuthenticationToken>
        {
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = token.AccessToken
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = token.RefreshToken
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.ExpiresIn,
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o")
            }
        });

        authProps.IsPersistent = false;

        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProps);

        return true;
    }
}
