namespace StudyResults.DTOs;

public class TimeSeriesDataDto
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
    public string MetricName { get; set; } = string.Empty;
}