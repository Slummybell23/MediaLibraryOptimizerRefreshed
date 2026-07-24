using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Server.Data;
using Server.Options;

namespace Server.Services;

public enum ScanState
{
    Idle,
    Running,
    Completed,
    Failed,
    Cancelled
}

public record ScanStatus(
    ScanState State,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    int FilesScanned,
    string? CurrentFile,
    string? ErrorMessage);

/// <summary>
/// Walks the media library and syncs what it finds into the MediaFiles table.
/// Registered as a singleton; at most one scan runs at a time.
/// </summary>
public class ScannerService(
    IDbContextFactory<AppDbContext> dbFactory,
    IOptions<StorageOptions> storageOptions,
    ILogger<ScannerService> logger)
{
    private readonly object _gate = new();
    private CancellationTokenSource? _cts;

    private ScanState _state = ScanState.Idle;
    private DateTime? _startedAtUtc;
    private DateTime? _completedAtUtc;
    private int _filesScanned;
    private string? _currentFile;
    private string? _errorMessage;

    public ScanStatus Status
    {
        get
        {
            lock (_gate)
            {
                return new ScanStatus(
                    _state, _startedAtUtc, _completedAtUtc,
                    _filesScanned, _currentFile, _errorMessage);
            }
        }
    }

    /// <summary>Kicks off a scan in the background. Returns false if one is already running.</summary>
    public bool TryStartScan()
    {
        lock (_gate)
        {
            if (_state == ScanState.Running)
            {
                return false;
            }

            _state = ScanState.Running;
            _startedAtUtc = DateTime.UtcNow;
            _completedAtUtc = null;
            _filesScanned = 0;
            _currentFile = null;
            _errorMessage = null;

            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }

        _ = Task.Run(() => RunScanAsync(_cts.Token));
        return true;
    }

    /// <summary>Requests cancellation of the running scan. No-op when idle.</summary>
    public void CancelScan()
    {
        lock (_gate)
        {
            _cts?.Cancel();
        }
    }

    private async Task RunScanAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ScanLibraryAsync(cancellationToken);
            Finish(ScanState.Completed);
        }
        catch (OperationCanceledException)
        {
            Finish(ScanState.Cancelled);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Library scan failed");
            lock (_gate)
            {
                _errorMessage = ex.Message;
            }
            Finish(ScanState.Failed);
        }
    }

    private async Task ScanLibraryAsync(CancellationToken cancellationToken)
    {
        var mediaRoot = Path.GetFullPath(storageOptions.Value.MediaPath);
        logger.LogInformation("Starting library scan of {MediaRoot}", mediaRoot);

        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);

        // TODO: implement the actual scan. Rough shape:
        //   1. Enumerate video files under mediaRoot (mkv, mp4, ...).
        //   2. Per file: cancellationToken.ThrowIfCancellationRequested(),
        //      probe with ffprobe (codec, container, Dolby Vision profile),
        //      then upsert a MediaFile row keyed on the path relative to mediaRoot.
        //   3. Call ReportProgress(relativePath) as each file is processed
        //      so the UI shows live progress.
        //   4. Handle rows whose files no longer exist on disk (delete or mark).
    }

    /// <summary>Call from ScanLibraryAsync as each file is processed.</summary>
    private void ReportProgress(string currentFile)
    {
        lock (_gate)
        {
            _filesScanned++;
            _currentFile = currentFile;
        }
    }

    private void Finish(ScanState finalState)
    {
        lock (_gate)
        {
            _state = finalState;
            _completedAtUtc = DateTime.UtcNow;
            _currentFile = null;
        }

        logger.LogInformation("Library scan finished: {State}", finalState);
    }
}
