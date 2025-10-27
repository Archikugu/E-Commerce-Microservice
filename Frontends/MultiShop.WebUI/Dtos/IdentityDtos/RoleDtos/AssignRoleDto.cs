namespace MultiShop.WebUI.Dtos.IdentityDtos.RoleDtos;

public class AssignRoleDto
{
    public string? UserId { get; set; }
    public string? UserFullName { get; set; }
    public List<AssignRoleItemDto> Roles { get; set; } = new();
}


