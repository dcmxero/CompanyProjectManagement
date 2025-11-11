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
    /// Gets the authentication configuration.
    /// </summary>
    public AuthConfig Auth { get; }

    public string ConfigDirectory { get; }

    public XmlConfigProvider(string configFilePath = "./config.xml")
    {
        ConfigDirectory = Path.GetDirectoryName(Path.GetFullPath(configFilePath)) ?? Directory.GetCurrentDirectory();

        if (!File.Exists(configFilePath))
        {
            Config = new XmlStorageConfig();
            Auth = new AuthConfig();
            MakeProjectsPathAbsolute();
            TryRegisterEncodingProvider(Config.EncodingName);
            return;
        }

        var doc = XDocument.Load(configFilePath, LoadOptions.PreserveWhitespace);
        var storage = doc.Root?.Element("storage");
        var auth = doc.Root?.Element("auth");

        var projectsPath = storage?.Element("projectsPath")?.Value?.Trim();
        var encodingName = storage?.Element("encoding")?.Value?.Trim();

        Config = new XmlStorageConfig
        {
            ProjectsPath = string.IsNullOrWhiteSpace(projectsPath) ? "./data/projects.xml" : projectsPath,
            EncodingName = string.IsNullOrWhiteSpace(encodingName) ? "windows-1250" : encodingName
        };

        Auth = new AuthConfig
        {
            Username = auth?.Element("username")?.Value ?? "admin",
            Password = auth?.Element("passwordHash")?.Value ?? string.Empty
        };

        MakeProjectsPathAbsolute();
        TryRegisterEncodingProvider(Config.EncodingName);
    }

    private void MakeProjectsPathAbsolute()
    {
        if (!Path.IsPathRooted(Config.ProjectsPath))
        {
            Config.ProjectsPath = Path.GetFullPath(Path.Combine(ConfigDirectory, Config.ProjectsPath));
        }
    }

    private static void TryRegisterEncodingProvider(string encodingName)
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _ = Encoding.GetEncoding(encodingName);
        }
        catch { }
    }
}
