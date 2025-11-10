using Domain.Models;

namespace Infrastructure.XmlStorage.Repositories;

/// <summary>
/// Provides access to projects stored in an XML file.
/// </summary>
public interface IXmlProjectRepository
{
    /// <summary>
    /// Gets all projects.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Projects.</returns>
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project by id.
    /// </summary>
    /// <param name="id">Project id.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Project or null.</returns>
    Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a project.
    /// </summary>
    /// <param name="project">Project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Upserted project.</returns>
    Task<Project> UpsertAsync(Project project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a project by id.
    /// </summary>
    /// <param name="id">Project id.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if deleted.</returns>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
