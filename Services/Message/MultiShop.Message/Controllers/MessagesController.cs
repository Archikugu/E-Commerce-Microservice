using Microsoft.AspNetCore.Mvc;
using MultiShop.Message.Dtos;
using MultiShop.Message.Services;

namespace MultiShop.Message.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _messageService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMessageDto dto)
    {
        if (dto == null) return BadRequest();
        await _messageService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateMessageDto dto)
    {
        if (dto == null || dto.MessageId <= 0) return BadRequest();
        var exists = await _messageService.GetByIdAsync(dto.MessageId);
        if (exists == null) return NotFound();
        await _messageService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var exists = await _messageService.GetByIdAsync(id);
        if (exists == null) return NotFound();
        await _messageService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("inbox/{receiverId}")]
    public async Task<IActionResult> GetInbox(string receiverId)
    {
        if (string.IsNullOrWhiteSpace(receiverId)) return BadRequest();
        var list = await _messageService.GetInboxAsync(receiverId);
        return Ok(list);
    }

    [HttpGet("sendbox/{senderId}")]
    public async Task<IActionResult> GetSendbox(string senderId)
    {
        if (string.IsNullOrWhiteSpace(senderId)) return BadRequest();
        var list = await _messageService.GetSendboxAsync(senderId);
        return Ok(list);
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var exists = await _messageService.GetByIdAsync(id);
        if (exists == null) return NotFound();
        await _messageService.MarkAsReadAsync(id);
        return NoContent();
    }
}


