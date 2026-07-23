using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/media-files")]
public class MediaFilesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaFile>>> GetAll()
    {
        return await db.MediaFiles
            .AsNoTracking()
            .OrderBy(f => f.FilePath)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MediaFile>> GetById(int id)
    {
        var file = await db.MediaFiles
            .AsNoTracking()
            .Include(f => f.Jobs)
            .FirstOrDefaultAsync(f => f.Id == id);

        return file is null ? NotFound() : file;
    }
}
