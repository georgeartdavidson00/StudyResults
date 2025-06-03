namespace StudyResults.DTOs;

public class UploadResponseDto
{
    public int BatchId { get; set; }
    public int RecordsProcessed { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}