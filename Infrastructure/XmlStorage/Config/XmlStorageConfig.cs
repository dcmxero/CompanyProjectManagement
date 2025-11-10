namespace Infrastructure.XmlStorage.Config;

/// <summary>
/// Represents XML storage configuration.
/// </summary>
public sealed class XmlStorageConfig
{
    /// <summary>
    /// Gets or sets the projects XML file path.
    /// </summary>
    public string ProjectsPath { get; set; } = "./data/projects.xml";

    /// <summary>
    /// Gets or sets the text encoding name (e.g., windows-1250).
    /// </summary>
    public string EncodingName { get; set; } = "windows-1250";
}
