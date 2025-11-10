using System.Text;
using System.Xml.Linq;

namespace Infrastructure.XmlStorage.Config;

/// <summary>
/// Loads XML storage configuration from config.xml.
/// </summary>
public sealed class XmlConfigProvider
{
    /// <summary>
    /// Gets the configuration loaded from XML.
    /// </summary>
    public XmlStorageConfig Config { get; }

    /// <summary>
    /// Initializes a new instance of the XmlConfigProvider class.
    /// </summary>
    /// <param name="configFilePath">Config file path.</param>
    public XmlConfigProvider(string configFilePath = "./config.xml")
    {
        if (!File.Exists(configFilePath))
        {
            Config = new XmlStorageConfig();
            return;
        }

        var doc = XDocument.Load(configFilePath, LoadOptions.PreserveWhitespace);
        var storage = doc.Root?.Element("storage");
        var projectsPath = storage?.Element("projectsPath")?.Value?.Trim();
        var encodingName = storage?.Element("encoding")?.Value?.Trim();

        Config = new XmlStorageConfig
        {
            ProjectsPath = string.IsNullOrWhiteSpace(projectsPath) ? "./data/projects.xml" : projectsPath,
            EncodingName = string.IsNullOrWhiteSpace(encodingName) ? "windows-1250" : encodingName
        };
        TryRegisterEncodingProvider(Config.EncodingName);
    }

    private static void TryRegisterEncodingProvider(string encodingName)
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _ = Encoding.GetEncoding(encodingName);
        }
        catch
        {
            // Fallback remains UTF-8 if invalid encoding is configured.
        }
    }
}
