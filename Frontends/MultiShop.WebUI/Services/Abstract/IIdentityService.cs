using MultiShop.WebUI.Dtos.IdentityDtos.LoginDtos;

namespace MultiShop.WebUI.Services.Abstract;

public interface IIdentityService
{
    Task<bool> SignIn(SignInDto signInDto);
}
