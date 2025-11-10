namespace Application.Dtos;

/// <summary>
/// Data transfer object for Project.
/// </summary>
public sealed class ProjectDto
{
    /// <summary>
    /// Gets or sets the project identifier.
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
