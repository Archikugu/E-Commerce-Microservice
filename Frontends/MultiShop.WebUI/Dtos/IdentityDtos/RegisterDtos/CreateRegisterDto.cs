namespace MultiShop.WebUI.Dtos.IdentityDtos.RegisterDtos;
public class CreateRegisterDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName.ToUpper()}";
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

