using InvestmentRace.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InvestmentRace.API.Data;

public class InvestmentRaceDbContext
    : IdentityDbContext<ApplicationUser>
{
    public InvestmentRaceDbContext(
        DbContextOptions<InvestmentRaceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Youth> Youths { get; set; }
}