namespace MultiShop.WebUI.Dtos.CommentDtos;

public class CreateCommentDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public string Email { get; set; }
    public string CommentDetail { get; set; }
    public int Rating { get; set; }
    public string ProductId { get; set; }
}
