using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Comment.Context;
using MultiShop.Comment.Dtos;
using MultiShop.Comment.Entities;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace MultiShop.Comment.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly MultiShopCommentContext _context;
    private readonly IMapper _mapper;

    public CommentsController(MultiShopCommentContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Comments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResultCommentDto>>> GetComments()
    {
        var comments = await _context.UserComments.ToListAsync();
        var result = _mapper.Map<List<ResultCommentDto>>(comments);
        return Ok(result);
    }

    // GET: api/Comments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ResultCommentDto>> GetComment(int id)
    {
        var comment = await _context.UserComments.FindAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        var result = _mapper.Map<ResultCommentDto>(comment);
        return Ok(result);
    }

    // GET: api/Comments/Product/{productId}
    [HttpGet("Product/{productId}")]
    public async Task<ActionResult<IEnumerable<ResultCommentDto>>> GetCommentsByProduct(string productId)
    {
        var comments = await _context.UserComments
            .Where(c => c.ProductId == productId && c.Status)
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();

        var result = _mapper.Map<List<ResultCommentDto>>(comments);
        return Ok(result);
    }

    // POST: api/Comments
    [HttpPost]
    public async Task<ActionResult<ResultCommentDto>> CreateComment(CreateCommentDto createCommentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var comment = _mapper.Map<UserComment>(createCommentDto);
        comment.CreatedDate = DateTime.UtcNow;
        comment.Status = true;

        _context.UserComments.Add(comment);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<ResultCommentDto>(comment);
        return CreatedAtAction(nameof(GetComment), new { id = comment.UserCommentId }, result);
    }

    // PUT: api/Comments/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto updateCommentDto)
    {
        if (id != updateCommentDto.UserCommentId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingComment = await _context.UserComments.FindAsync(id);
        if (existingComment == null)
        {
            return NotFound();
        }

        _mapper.Map(updateCommentDto, existingComment);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CommentExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Comments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.UserComments.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        _context.UserComments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PATCH: api/Comments/5/Status
    [HttpPatch("{id}/Status")]
    public async Task<IActionResult> UpdateCommentStatus(int id, [FromBody] bool status)
    {
        var comment = await _context.UserComments.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        comment.Status = status;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CommentExists(int id)
    {
        return _context.UserComments.Any(e => e.UserCommentId == id);
    }
}
