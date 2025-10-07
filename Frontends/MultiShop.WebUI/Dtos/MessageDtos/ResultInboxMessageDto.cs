namespace MultiShop.WebUI.Dtos.MessageDtos;

public class ResultInboxMessageDto
{
    public int MessageId { get; set; }
    public string? SenderId { get; set; }
    public string? Subject { get; set; }
    public string? MessageDetail { get; set; }
    public bool IsRead { get; set; }
    public DateTime MessageDate { get; set; }
}


