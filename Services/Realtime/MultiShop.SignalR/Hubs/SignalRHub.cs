using Microsoft.AspNetCore.SignalR;
using MultiShop.SignalR.Services.SignalRCommentServices;
using MultiShop.SignalR.Services.SignalRMessageServices;

namespace MultiShop.SignalR.Hubs;

public class SignalRHub : Hub
{
    private readonly ISignalRCommentService _signalRCommentService;
    private readonly ISignalRMessageService _signalRMessageService;

    public SignalRHub(ISignalRMessageService signalRMessageService, ISignalRCommentService signalRCommentService)
    {
        _signalRMessageService = signalRMessageService;
        _signalRCommentService = signalRCommentService;
    }

    public async Task SendStatisticCount()
    {
        var getTotalCommentCount = await _signalRCommentService.GetTotalCommentCountAsync();
        await Clients.All.SendAsync("ReceiveCommentCount", getTotalCommentCount);

        // Genel toplam mesaj sayısı
        var getTotalMessageCount = await _signalRMessageService.GetTotalMessageCountAsync();
        await Clients.All.SendAsync("ReceiveMessageCount", getTotalMessageCount);
    }

    public async Task SendStatisticCountFor(string receiverId)
    {
        if (string.IsNullOrWhiteSpace(receiverId))
        {
            await SendStatisticCount();
            return;
        }

        var getTotalCommentCount = await _signalRCommentService.GetTotalCommentCountAsync();
        await Clients.Caller.SendAsync("ReceiveCommentCount", getTotalCommentCount);

        var getTotalMessageCount = await _signalRMessageService.GetTotalMessageCountReciverIdAsync(receiverId);
        await Clients.Caller.SendAsync("ReceiveMessageCount", getTotalMessageCount);
    }
}

