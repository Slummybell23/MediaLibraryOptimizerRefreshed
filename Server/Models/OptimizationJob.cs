namespace Server.Models;

public enum JobType
{
    /// <summary>Re-encode the video stream to near-lossless AV1 to reclaim disk space.</summary>
    Av1Encode,

    /// <summary>Remux Dolby Vision profile 7 to profile 8 without re-encoding.</summary>
    DolbyVisionRemux
}

public enum JobStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}

public class OptimizationJob
{
    public int Id { get; set; }

    public int MediaFileId { get; set; }

    public MediaFile MediaFile { get; set; } = null!;

    public JobType Type { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Pending;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public string? ErrorMessage { get; set; }

    public long? OriginalSizeBytes { get; set; }

    public long? ResultSizeBytes { get; set; }
}
