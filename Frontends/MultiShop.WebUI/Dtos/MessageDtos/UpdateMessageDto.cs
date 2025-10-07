namespace MultiShop.WebUI.Dtos.MessageDtos;

public class UpdateMessageDto
{
    public int MessageId { get; set; }
    public string? Subject { get; set; }
    public string? MessageDetail { get; set; }
    public bool IsRead { get; set; }
}


