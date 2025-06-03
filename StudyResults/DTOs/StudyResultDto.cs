namespace StudyResults.DTOs;

public class StudyResultDto
{
    public int Id { get; set; }
    public string ParticipantId { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public double MetricValue { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
}