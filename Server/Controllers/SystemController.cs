using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Server.Data;
using Server.Models;
using Server.Options;

namespace Server.Controllers;

[ApiController]
[Route("api/system")]
public class SystemController(AppDbContext db, IOptions<StorageOptions> storage) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var mediaPath = Path.GetFullPath(storage.Value.MediaPath);
        var appDataPath = Path.GetFullPath(storage.Value.AppDataPath);

        return Ok(new
        {
            MediaPath = mediaPath,
            MediaPathExists = Directory.Exists(mediaPath),
            AppDataPath = appDataPath,
            MediaFileCount = await db.MediaFiles.CountAsync(),
            PendingJobCount = await db.Jobs.CountAsync(j => j.Status == JobStatus.Pending),
            RunningJobCount = await db.Jobs.CountAsync(j => j.Status == JobStatus.Running)
        });
    }
}
