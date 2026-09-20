namespace InvestmentRace.API.Models;

public class SocialWorkerYouth
{
    public int Id { get; set; }

    public string SocialWorkerId { get; set; } = string.Empty;

    public int YouthId { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    public ApplicationUser? SocialWorker { get; set; }

    public Youth? Youth { get; set; }
}