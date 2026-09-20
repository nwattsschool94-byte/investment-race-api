using InvestmentRace.API.DTOs;
using InvestmentRace.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InvestmentRace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
private readonly UserManager<ApplicationUser> _userManager;
private readonly IConfiguration _configuration;

public AuthController(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration)
{
    _userManager = userManager;
    _configuration = configuration;
}
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return BadRequest(new
            {
                message = "An account with this email already exists."
            });
        }

var user = new ApplicationUser
{
    FirstName = request.FirstName,
    LastName = request.LastName,
    Email = request.Email,
    UserName = request.Email,
    IsApproved = false
};

        var result = await _userManager.CreateAsync(
            user,
            request.Password
        );

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(error => error.Description)
            });
        }

        return Ok(new
        {
            message = "Account created successfully.",
            user = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email
            }
        });
    }

[HttpPost("login")]
public async Task<IActionResult> Login(LoginDto request)
{
    var user = await _userManager.FindByEmailAsync(request.Email);

    if (user == null)
    {
        return Unauthorized(new
        {
            message = "Invalid email or password."
        });
    }

    var passwordValid = await _userManager.CheckPasswordAsync(
        user,
        request.Password
    );

    if (!passwordValid)
    {
        return Unauthorized(new
        {
            message = "Invalid email or password."
        });
    }

    // Account exists, but Level Up has not approved it yet
    if (!user.IsApproved)
    {
        return StatusCode(403, new
        {
            message = "Your account is waiting for approval from Level Up."
        });
    }

    var roles = await _userManager.GetRolesAsync(user);

var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
};

foreach (var role in roles)
{
    claims.Add(new Claim(ClaimTypes.Role, role));
}

var jwtKey = _configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

var key = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtKey)
);

var credentials = new SigningCredentials(
    key,
    SecurityAlgorithms.HmacSha256
);

var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddHours(8),
    signingCredentials: credentials
);

var tokenString = new JwtSecurityTokenHandler()
    .WriteToken(token);

return Ok(new
{
    message = "Login successful.",
    token = tokenString,
    user = new
    {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.IsApproved,
        roles
    }
});
}
}