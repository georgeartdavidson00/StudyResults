using System.ComponentModel.DataAnnotations;

namespace StudyResults.Models;

public class StudyResult
{
    public int Id { get; set; }
        
    [Required]
    public string ParticipantId { get; set; } = string.Empty;
        
    [Required]
    public string MetricName { get; set; } = string.Empty;
        
    public double MetricValue { get; set; }
        
    public DateTime Timestamp { get; set; }
        
    public string? Category { get; set; }
        
    public string? Notes { get; set; }
        
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    public int UploadBatchId { get; set; }
    public UploadBatch UploadBatch { get; set; } = null!;
}