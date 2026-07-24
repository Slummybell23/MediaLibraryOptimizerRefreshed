namespace Server.Options;

public class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Directory holding the SQLite database and other app state.</summary>
    public string AppDataPath { get; set; } = "appdata";

    /// <summary>Root of the media library to scan.</summary>
    public string MediaPath { get; set; } = "media";
}
