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

        // Refresh token, ilk token hangi client ile alındıysa o client ile yenilenmelidir.
        RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest
        {
            ClientId = _clientSettings.MultiShopAdminClient.ClientId,
            ClientSecret = _clientSettings.MultiShopAdminClient.ClientSecret,
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
        try
        {
            // IdentityServer'ın custom login endpoint'ini kullan
            var loginRequest = new
            {
                LoginIdentifier = signInDto.UserName, // Email veya Username
                Password = signInDto.Password
            };

            var loginResponse = await _httpClient.PostAsJsonAsync($"{_serviceAPISettings.IdentityServerUrl}/api/Logins", loginRequest);
            
            if (loginResponse.IsSuccessStatusCode)
            {
                var tokenResponse = await loginResponse.Content.ReadAsStringAsync();
                // Email ile girişte gerçek kullanıcı adını çıkarıp ROPC ile gerçek access token al
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var fallbackClaims = new List<Claim>();
                try
                {
                    var jsonToken = handler.ReadJwtToken(tokenResponse);
                    var preferredUsername = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                    if (!string.IsNullOrWhiteSpace(preferredUsername))
                    {
                        signInDto.UserName = preferredUsername;
                    }
                    // Fallback için önemli claim'leri sakla
                    if (jsonToken != null)
                    {
                        foreach (var c in jsonToken.Claims)
                        {
                            if (c.Type == "given_name" || c.Type == "family_name" || c.Type == "email" || c.Type == "name" || c.Type == "preferred_username")
                            {
                                fallbackClaims.Add(new Claim(c.Type, c.Value));
                            }
                        }
                    }
                }
                catch { }

                var ropResult = await SignInWithResourceOwnerPassword(signInDto, fallbackClaims);
                if (!ropResult)
                {
                    throw new Exception("Login failed. Unable to obtain access token.");
                }
                return true;
            }
            else
            {
                var errorContent = await loginResponse.Content.ReadAsStringAsync();
                var statusCode = loginResponse.StatusCode;
                
                // Kullanıcı dostu hata mesajları
                if (statusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    if (errorContent.Contains("User not found"))
                    {
                        throw new Exception("Invalid username, email or password.");
                    }
                    else if (errorContent.Contains("Invalid login attempt"))
                    {
                        throw new Exception("Invalid username, email or password.");
                    }
                    else
                    {
                        throw new Exception("Login failed. Please check your credentials.");
                    }
                }
                else
                {
                    throw new Exception("Login failed. Please try again later.");
                }
            }
        }
        catch (Exception ex)
        {
            throw ex; // Exception'ı olduğu gibi fırlat, ek prefix ekleme
        }
    }

    private async Task<bool> SignInWithResourceOwnerPassword(SignInDto signInDto, List<Claim>? fallbackClaims = null)
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
            Address = discoveryEndPoint.TokenEndpoint,
            Scope = "openid profile email BasketFullPermission DiscountFullPermission OcelotFullPermission offline_access"
        };

        var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
        
        if (token.IsError || string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new Exception($"Token request failed: {token.Error} - {token.ErrorDescription}");
        }

        var userInfo = await _httpClient.GetUserInfoAsync(new UserInfoRequest
        {
            Address = discoveryEndPoint.UserInfoEndpoint,
            Token = token.AccessToken
        });

        if (userInfo.IsError)
        {
            throw new Exception($"User info request failed: {userInfo.Error}");
        }

        // userinfo + fallback claim birleştirme
        var mergedClaims = new List<Claim>(userInfo.Claims);
        if (fallbackClaims != null && fallbackClaims.Count > 0)
        {
            string? Get(string type) => mergedClaims.FirstOrDefault(c => c.Type == type)?.Value;
            void AddIfMissing(string type)
            {
                if (string.IsNullOrWhiteSpace(Get(type)))
                {
                    var fc = fallbackClaims.FirstOrDefault(c => c.Type == type);
                    if (fc != null) mergedClaims.Add(new Claim(fc.Type, fc.Value));
                }
            }
            AddIfMissing("preferred_username");
            AddIfMissing("email");
            AddIfMissing("given_name");
            AddIfMissing("family_name");
            AddIfMissing("name");
        }

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(mergedClaims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

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

    private async Task SetTokenInCookie(string tokenResponse)
    {
        // IdentityServer'dan gelen token response'u parse et
        try
        {
            // Debug: Token response'u logla
            Console.WriteLine($"Token Response: {tokenResponse}");
            
            // JWT token'ı decode et ve claims'leri al
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(tokenResponse);
            
            var claims = new List<Claim>();
            
            // JWT token'dan claims'leri al
            foreach (var claim in jsonToken.Claims)
            {
                claims.Add(new Claim(claim.Type, claim.Value));
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }

            // Store raw access token as a claim for fallback
            claims.Add(new Claim("access_token", tokenResponse));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = false
            };

            // Also store tokens in auth properties so GetTokenAsync works
            authProps.StoreTokens(new List<AuthenticationToken>
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResponse
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.ExpiresIn,
                    Value = DateTime.Now.AddHours(1).ToString("o")
                }
            });

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProps);
        }
        catch (Exception ex)
        {
            // Debug: Hata mesajını logla
            Console.WriteLine($"Token parse error: {ex.Message}");
            
            // Token parse edilemezse, basit claims ile devam et
            var claims = new List<Claim>
            {
                new Claim("sub", "user_id"),
                new Claim("name", "Kullanıcı"),
                new Claim("preferred_username", "kullanici"),
                new Claim("email", "kullanici@example.com"),
                new Claim("given_name", "Kullanıcı"),
                new Claim("family_name", "Adı")
            };

            // Fallback: also store token as claim so handler can read it
            claims.Add(new Claim("access_token", tokenResponse));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = false
            };
            authProps.StoreTokens(new List<AuthenticationToken>
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResponse
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.ExpiresIn,
                    Value = DateTime.Now.AddHours(1).ToString("o")
                }
            });

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProps);
        }
    }

    private async Task<List<Claim>> GetUserInfoFromIdentityServer(string token)
    {
        try
        {
            // IdentityServer'dan kullanıcı bilgilerini al
            var userInfoRequest = new UserInfoRequest
            {
                Address = $"{_serviceAPISettings.IdentityServerUrl}/connect/userinfo",
                Token = token
            };

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);
            
            if (!userInfo.IsError)
            {
                return userInfo.Claims.ToList();
            }
        }
        catch (Exception)
        {
            // Hata durumunda boş liste döndür
        }

        return new List<Claim>();
    }
}
