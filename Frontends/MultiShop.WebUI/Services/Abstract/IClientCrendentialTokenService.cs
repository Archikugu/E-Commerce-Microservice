namespace MultiShop.WebUI.Services.Abstract
{
    public interface IClientCrendentialTokenService
    {
        Task<string> GetToken();
    }
}
