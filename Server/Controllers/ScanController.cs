using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/scan")]
public class ScanController(ScannerService scanner) : ControllerBase
{
    [HttpGet("status")]
    public ActionResult<ScanStatus> GetStatus() => scanner.Status;

    [HttpPost]
    public ActionResult<ScanStatus> Start()
    {
        if (!scanner.TryStartScan())
        {
            return Conflict("A scan is already running.");
        }

        return AcceptedAtAction(nameof(GetStatus), scanner.Status);
    }

    [HttpPost("cancel")]
    public ActionResult<ScanStatus> Cancel()
    {
        scanner.CancelScan();
        return scanner.Status;
    }
}
