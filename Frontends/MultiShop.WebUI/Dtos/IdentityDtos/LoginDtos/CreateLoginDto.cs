namespace MultiShop.WebUI.Dtos.IdentityDtos.LoginDtos;

public class CreateLoginDto
{
    public string LoginIdentifier { get; set; } // Username veya Email
    public string Password { get; set; }
}
