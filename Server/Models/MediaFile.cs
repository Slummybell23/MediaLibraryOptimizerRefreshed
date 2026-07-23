namespace Server.Models;

public enum MediaFileStatus
{
    Discovered,
    Analyzed,
    Queued,
    Processing,
    Optimized,
    Failed,
    Skipped
}

public class MediaFile
{
    public int Id { get; set; }

    /// <summary>Path relative to the media library root (the /media mount in Docker).</summary>
    public required string FilePath { get; set; }

    public required string FileName { get; set; }

    public long FileSizeBytes { get; set; }

    /// <summary>Container format, e.g. "mkv", "mp4".</summary>
    public string? Container { get; set; }

    /// <summary>Video codec, e.g. "h264", "hevc", "av1".</summary>
    public string? VideoCodec { get; set; }

    /// <summary>Dolby Vision profile if present (7 needs remuxing to 8 for broad compatibility).</summary>
    public int? DolbyVisionProfile { get; set; }

    public MediaFileStatus Status { get; set; } = MediaFileStatus.Discovered;

    public DateTime DiscoveredAtUtc { get; set; }

    public DateTime? LastScannedAtUtc { get; set; }

    public List<OptimizationJob> Jobs { get; set; } = [];
}
