using InvestmentRace.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentRace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/admin/pending
    [HttpGet("pending")]
    public IActionResult GetPendingUsers()
    {
        var users = _userManager.Users
            .Where(user => !user.IsApproved)
            .Select(user => new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email
            })
            .ToList();

        return Ok(users);
    }

    // PUT: api/admin/approve/{id}
    [HttpPut("approve/{id}")]
    public async Task<IActionResult> ApproveUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        if (user.IsApproved)
        {
            return BadRequest(new
            {
                message = "This account is already approved."
            });
        }

        user.IsApproved = true;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return BadRequest(new
            {
                errors = updateResult.Errors
                    .Select(error => error.Description)
            });
        }

        // Approval makes this account a SocialWorker
        if (!await _userManager.IsInRoleAsync(user, "SocialWorker"))
        {
            var roleResult =
                await _userManager.AddToRoleAsync(user, "SocialWorker");

            if (!roleResult.Succeeded)
            {
                // Don't leave the user partially approved
                user.IsApproved = false;
                await _userManager.UpdateAsync(user);

                return BadRequest(new
                {
                    errors = roleResult.Errors
                        .Select(error => error.Description)
                });
            }
        }

        return Ok(new
        {
            message = "Social worker approved successfully."
        });
    }
}