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
public class YouthController : ControllerBase
{
    private readonly InvestmentRaceDbContext _context;

    public YouthController(InvestmentRaceDbContext context)
    {
        _context = context;
    }

    // GET: api/Youth
    // Returns the logged-in social worker's youth list.
    [HttpGet]
    public async Task<IActionResult> GetYouths()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var youths = await _context.SocialWorkerYouths
            .Where(x => x.SocialWorkerId == userId)
            .OrderByDescending(x => x.DateAdded)
            .Select(x => new
            {
                x.Youth!.Id,
                x.Youth.FirstName,
                x.Youth.LastName,
                x.Youth.CreatedAt,
                x.DateAdded
            })
            .ToListAsync();

        return Ok(youths);
    }


    // POST: api/Youth
    // Creates a new youth and adds them to the
    // logged-in social worker's list.
    [HttpPost]
    public async Task<IActionResult> CreateYouth(
        CreateYouthDto request)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        if (
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName)
        )
        {
            return BadRequest(new
            {
                message = "First and last name are required."
            });
        }

        var youth = new Youth
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim()
        };

        _context.Youths.Add(youth);

        await _context.SaveChangesAsync();

        var assignment = new SocialWorkerYouth
        {
            SocialWorkerId = userId,
            YouthId = youth.Id
        };

        _context.SocialWorkerYouths.Add(assignment);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Youth created and added to your list.",
            youth.Id,
            youth.FirstName,
            youth.LastName,
            youth.CreatedAt
        });
    }


    // GET: api/Youth/search?name=john
    // Searches the master youth list.
    [HttpGet("search")]
    public async Task<IActionResult> SearchYouths(
        [FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Ok(Array.Empty<object>());
        }

        var search = name.Trim();

        var youths = await _context.Youths
            .Where(y =>
                y.FirstName.Contains(search) ||
                y.LastName.Contains(search) ||
                (y.FirstName + " " + y.LastName)
                    .Contains(search)
            )
            .OrderBy(y => y.LastName)
            .ThenBy(y => y.FirstName)
            .Take(20)
            .Select(y => new
            {
                y.Id,
                y.FirstName,
                y.LastName,
                y.CreatedAt
            })
            .ToListAsync();

        return Ok(youths);
    }


    // POST: api/Youth/5/add
    // Adds an existing youth to the logged-in
    // social worker's personal list.
    [HttpPost("{id}/add")]
    public async Task<IActionResult> AddExistingYouth(
        int id)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var youth = await _context.Youths.FindAsync(id);

        if (youth == null)
        {
            return NotFound(new
            {
                message = "Youth not found."
            });
        }

        var alreadyAdded =
            await _context.SocialWorkerYouths.AnyAsync(
                x =>
                    x.SocialWorkerId == userId &&
                    x.YouthId == id
            );

        if (alreadyAdded)
        {
            return BadRequest(new
            {
                message =
                    "This youth is already in your list."
            });
        }

        var assignment = new SocialWorkerYouth
        {
            SocialWorkerId = userId,
            YouthId = id
        };

        _context.SocialWorkerYouths.Add(assignment);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Youth added to your list."
        });
    }

    // GET: api/Youth/5
[HttpGet("{id}")]
public async Task<IActionResult> GetYouth(int id)
{
    var userId = User.FindFirstValue(
        ClaimTypes.NameIdentifier
    );

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized();
    }

    var isAdmin = User.IsInRole("Admin");

    var hasAccess = isAdmin ||
        await _context.SocialWorkerYouths.AnyAsync(
            x =>
                x.SocialWorkerId == userId &&
                x.YouthId == id
        );

    if (!hasAccess)
    {
        return Forbid();
    }

    var youth = await _context.Youths
        .Where(y => y.Id == id)
        .Select(y => new
        {
            y.Id,
            y.FirstName,
            y.LastName,
            y.CreatedAt
        })
        .FirstOrDefaultAsync();

    if (youth == null)
    {
        return NotFound(new
        {
            message = "Youth not found."
        });
    }

    return Ok(youth);
}
}