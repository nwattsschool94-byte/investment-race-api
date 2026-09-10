using InvestmentRace.API.Data;
using InvestmentRace.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentRace.API.DTOs;

namespace InvestmentRace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YouthController : ControllerBase
{
    private readonly InvestmentRaceDbContext _context;

    public YouthController(InvestmentRaceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Youth>>> GetYouths()
    {
        var youths = await _context.Youths.ToListAsync();

        return Ok(youths);
    }

[HttpPost]
public async Task<ActionResult<Youth>> CreateYouth(CreateYouthDto youthDto)
{
    var youth = new Youth
    {
        FirstName = youthDto.FirstName,
        LastName = youthDto.LastName
    };

    _context.Youths.Add(youth);

    await _context.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetYouths),
        new { id = youth.Id },
        youth);
}
}