using Microsoft.AspNetCore.Identity;

namespace InvestmentRace.API.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // New accounts must be approved by Level Up
    public bool IsApproved { get; set; } = false;
}