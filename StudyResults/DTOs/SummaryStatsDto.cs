namespace StudyResults.DTOs;

public class SummaryStatsDto
{
    public string MetricName { get; set; } = string.Empty;
    public double Average { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public int Count { get; set; }
    public double StandardDeviation { get; set; }
}