namespace MultiShop.IdentityServer.Dtos;

public class UserRegisterDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName.ToUpper()}";
    public string Password { get; set; }
}
