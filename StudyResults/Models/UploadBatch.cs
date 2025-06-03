using System.ComponentModel.DataAnnotations;

namespace StudyResults.Models;

public class UploadBatch
{
    public int Id { get; set; }
        
    [Required]
    public string FileName { get; set; } = string.Empty;
        
    public long FileSizeBytes { get; set; }
        
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        
    public int RecordCount { get; set; }
        
    public UploadStatus Status { get; set; } = UploadStatus.Processing;
        
    public string? ErrorMessage { get; set; }
        
    public ICollection<StudyResult> StudyResults { get; set; } = new List<StudyResult>();
    
    public enum UploadStatus
    {
        Processing,
        Completed,
        Failed
    }

}