using System.ComponentModel.DataAnnotations;

namespace MultiShop.Comment.Entities;

public class UserComment
{
    public int UserCommentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => FirstName + " " + LastName.ToUpper();
    public string? ImageUrl { get; set; }
    public string Email { get; set; }
    public string CommentDetail { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool Status { get; set; }
    public string ProductId { get; set; }
}
