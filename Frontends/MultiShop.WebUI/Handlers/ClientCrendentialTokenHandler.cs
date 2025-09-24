
using System.Net;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Handlers;

public class ClientCrendentialTokenHandler : DelegatingHandler
{
    private readonly IClientCrendentialTokenService _clientCrendentialTokenService;

    public ClientCrendentialTokenHandler(IClientCrendentialTokenService clientCrendentialTokenService)
    {
        _clientCrendentialTokenService = clientCrendentialTokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _clientCrendentialTokenService.GetToken());
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            //Error Message
        }
        return response;
    }
}
