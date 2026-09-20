namespace InvestmentRace.API.Models;

public class YouthNote
{
    public int Id { get; set; }

    public int YouthId { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Youth? Youth { get; set; }

    public ApplicationUser? CreatedByUser { get; set; }
}