using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/system")]
public class SystemController(AppDbContext db, IConfiguration configuration) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var mediaPath = Path.GetFullPath(configuration["Storage:MediaPath"] ?? "media");
        var appDataPath = Path.GetFullPath(configuration["Storage:AppDataPath"] ?? "appdata");

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
