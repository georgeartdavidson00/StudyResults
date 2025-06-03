using Microsoft.EntityFrameworkCore;
using StudyResults.Models;

namespace StudyResults.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<StudyResult> StudyResults { get; set; }
    public DbSet<UploadBatch> UploadBatches { get; set; }
}