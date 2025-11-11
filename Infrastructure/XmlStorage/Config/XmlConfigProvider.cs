using System.Text;
using System.Xml.Linq;

namespace Infrastructure.XmlStorage.Config;

/// <summary>
/// Loads and parses XML storage configuration from the specified config.xml file.
/// </summary>
public sealed class XmlConfigProvider
{
    /// <summary>
    /// Gets the storage configuration loaded from XML.
    /// </summary>
    public XmlStorageConfig Config { get; }

    /// <summary>
    /// Gets the authentication configuration loaded from XML.
    /// </summary>
    public AuthConfig Auth { get; }

    /// <summary>
    /// Gets the directory path of the configuration file.
    /// </summary>
    public string ConfigDirectory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlConfigProvider"/> class and loads configuration from XML.
    /// </summary>
    /// <param name="configFilePath">Path to the configuration XML file. Defaults to './config.xml'.</param>
    public XmlConfigProvider(string configFilePath = "./config.xml")
    {
        ConfigDirectory = Directory.GetCurrentDirectory();

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

    /// <summary>
    /// Ensures the projects path is an absolute path.
    /// </summary>
    private void MakeProjectsPathAbsolute()
    {
        if (!Path.IsPathRooted(Config.ProjectsPath))
        {
            Config.ProjectsPath = Path.GetFullPath(Path.Combine(ConfigDirectory, Config.ProjectsPath));
        }
    }

    /// <summary>
    /// Registers the encoding provider and attempts to load the specified encoding.
    /// </summary>
    /// <param name="encodingName">Name of the encoding to register and verify.</param>
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