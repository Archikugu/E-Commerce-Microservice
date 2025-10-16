namespace MultiShop.SignalR.Services.SignalRMessageServices
{
    public interface ISignalRMessageService
    {
        Task<int> GetTotalMessageCountReciverIdAsync(string id);
        Task<int> GetTotalMessageCountAsync();
    }
}
