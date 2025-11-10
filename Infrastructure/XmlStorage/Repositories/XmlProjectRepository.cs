using Domain.Models;
using Infrastructure.XmlStorage.Config;
using System.Text;
using System.Xml.Linq;

namespace Infrastructure.XmlStorage.Repositories;

/// <summary>
/// XML-backed implementation of IXmlProjectRepository.
/// </summary>
public sealed class XmlProjectRepository : IXmlProjectRepository
{
    private readonly string _filePath;
    private readonly Encoding _encoding;

    /// <summary>
    /// Initializes a new instance of the XmlProjectRepository class.
    /// </summary>
    /// <param name="configProvider">Config provider.</param>
    public XmlProjectRepository(XmlConfigProvider configProvider)
    {
        _filePath = configProvider.Config.ProjectsPath;
        TryEnsureDirectory(_filePath);
        _encoding = GetEncoding(configProvider.Config.EncodingName);
        if (!File.Exists(_filePath))
        {
            var seed = new XDocument(new XElement("projects"));
            using var writer = new StreamWriter(_filePath, false, _encoding);
            seed.Save(writer);
        }
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var doc = Load();
        var items = doc.Root!.Elements("project")
            .Select(e => new Project
            {
                Id = (string?)e.Attribute("id") ?? string.Empty,
                Name = e.Element("name")?.Value ?? string.Empty,
                Abbreviation = e.Element("abbreviation")?.Value ?? string.Empty,
                Customer = e.Element("customer")?.Value ?? string.Empty
            })
            .ToList()
            .AsReadOnly();
        return Task.FromResult<IReadOnlyList<Project>>(items);
    }

    /// <inheritdoc/>
    public Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var doc = Load();
        var e = doc.Root!.Elements("project").FirstOrDefault(x => (string?)x.Attribute("id") == id);
        if (e is null)
        {
            return Task.FromResult<Project?>(null);
        }
        var p = new Project
        {
            Id = (string?)e.Attribute("id") ?? string.Empty,
            Name = e.Element("name")?.Value ?? string.Empty,
            Abbreviation = e.Element("abbreviation")?.Value ?? string.Empty,
            Customer = e.Element("customer")?.Value ?? string.Empty
        };
        return Task.FromResult<Project?>(p);
    }

    /// <inheritdoc/>
    public Task<Project> UpsertAsync(Project project, CancellationToken cancellationToken = default)
    {
        var doc = Load();
        var root = doc.Root!;
        var existing = root.Elements("project").FirstOrDefault(x => (string?)x.Attribute("id") == project.Id);
        if (existing is null)
        {
            var node = new XElement("project",
                new XAttribute("id", project.Id),
                new XElement("name", project.Name),
                new XElement("abbreviation", project.Abbreviation),
                new XElement("customer", project.Customer));
            root.Add(node);
        }
        else
        {
            existing.SetElementValue("name", project.Name);
            existing.SetElementValue("abbreviation", project.Abbreviation);
            existing.SetElementValue("customer", project.Customer);
        }
        Save(doc);
        return Task.FromResult(project);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var doc = Load();
        var root = doc.Root!;
        var e = root.Elements("project").FirstOrDefault(x => (string?)x.Attribute("id") == id);
        if (e is null)
        {
            return Task.FromResult(false);
        }
        e.Remove();
        Save(doc);
        return Task.FromResult(true);
    }

    private XDocument Load()
    {
        using var reader = new StreamReader(_filePath, _encoding, true);
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    }

    private void Save(XDocument doc)
    {
        using var writer = new StreamWriter(_filePath, false, _encoding);
        doc.Save(writer);
    }

    private static void TryEnsureDirectory(string filePath)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private static Encoding GetEncoding(string encodingName)
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(encodingName);
        }
        catch
        {
            return new UTF8Encoding(false);
        }
    }
}
