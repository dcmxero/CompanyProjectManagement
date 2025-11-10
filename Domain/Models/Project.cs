namespace Domain.Models;

/// <summary>
/// Represents a company project.
/// </summary>
public class Project
{
    /// <summary>
    /// Gets or sets the unique identifier from the XML attribute 'id'.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project abbreviation.
    /// </summary>
    public string Abbreviation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string Customer { get; set; } = string.Empty;
}
