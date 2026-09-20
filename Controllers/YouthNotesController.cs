using System.Security.Claims;
using InvestmentRace.API.Data;
using InvestmentRace.API.DTOs;
using InvestmentRace.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestmentRace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SocialWorker")]
public class YouthNotesController : ControllerBase
{
    private readonly InvestmentRaceDbContext _context;

    public YouthNotesController(
        InvestmentRaceDbContext context)
    {
        _context = context;
    }

    // GET: api/YouthNotes/youth/5
[HttpGet("youth/{youthId}")]
public async Task<IActionResult> GetYouthNotes(int youthId)
{
    var userId = User.FindFirstValue(
        ClaimTypes.NameIdentifier
    );

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized();
    }

    var youthExists = await _context.Youths
        .AnyAsync(y => y.Id == youthId);

    if (!youthExists)
    {
        return NotFound(new
        {
            message = "Youth not found."
        });
    }

    var isAdmin = User.IsInRole("Admin");

    var hasAccess = isAdmin ||
        await _context.SocialWorkerYouths
            .AnyAsync(x =>
                x.SocialWorkerId == userId &&
                x.YouthId == youthId
            );

    if (!hasAccess)
    {
        return Forbid();
    }

    var notes = await _context.YouthNotes
        .Where(n => n.YouthId == youthId)
        .OrderByDescending(n => n.CreatedAt)
        .Select(n => new
        {
            n.Id,
            n.YouthId,
            n.Note,
            n.CreatedAt,

            CreatedBy = n.CreatedByUser == null
                ? "Unknown"
                : n.CreatedByUser.FirstName + " " +
                  n.CreatedByUser.LastName
        })
        .ToListAsync();

    return Ok(notes);
}

    // POST: api/YouthNotes
    [HttpPost]
    public async Task<IActionResult> CreateYouthNote(
        CreateYouthNoteDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest(new
            {
                message = "A note is required."
            });
        }

        var youth = await _context.Youths
            .FindAsync(request.YouthId);

        if (youth == null)
        {
            return NotFound(new
            {
                message = "Youth not found."
            });
        }

        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

            // 3. CHECK PERMISSION HERE
    var isAdmin = User.IsInRole("Admin");

    var hasAccess = isAdmin ||
        await _context.SocialWorkerYouths
            .AnyAsync(x =>
                x.SocialWorkerId == userId &&
                x.YouthId == request.YouthId
            );

    if (!hasAccess)
    {
        return Forbid();
    }

        // 4. Now it's safe to create the note
        var note = new YouthNote
        {
            YouthId = request.YouthId,
            CreatedByUserId = userId,
            Note = request.Note.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.YouthNotes.Add(note);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Note added successfully.",
            note.Id,
            note.YouthId,
            note.Note,
            note.CreatedAt
        });
    }
}