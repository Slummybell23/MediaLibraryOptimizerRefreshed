using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

public record CreateJobRequest(int MediaFileId, JobType Type);

[ApiController]
[Route("api/jobs")]
public class JobsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OptimizationJob>>> GetAll([FromQuery] JobStatus? status)
    {
        var query = db.Jobs.AsNoTracking().Include(j => j.MediaFile).AsQueryable();

        if (status is not null)
        {
            query = query.Where(j => j.Status == status);
        }

        return await query.OrderByDescending(j => j.CreatedAtUtc).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OptimizationJob>> GetById(int id)
    {
        var job = await db.Jobs
            .AsNoTracking()
            .Include(j => j.MediaFile)
            .FirstOrDefaultAsync(j => j.Id == id);

        return job is null ? NotFound() : job;
    }

    [HttpPost]
    public async Task<ActionResult<OptimizationJob>> Create(CreateJobRequest request)
    {
        var file = await db.MediaFiles.FindAsync(request.MediaFileId);
        if (file is null)
        {
            return NotFound($"Media file {request.MediaFileId} does not exist.");
        }

        var job = new OptimizationJob
        {
            MediaFileId = file.Id,
            Type = request.Type,
            CreatedAtUtc = DateTime.UtcNow,
            OriginalSizeBytes = file.FileSizeBytes
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }
}
