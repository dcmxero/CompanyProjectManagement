using Application.Dtos;

namespace Application.Services;

/// <summary>
/// Provides project application operations.
/// </summary>
/// <param name="cancellationToken">Optional cancellation token.</param>
public interface IProjectAppService
{
    /// <summary>
    /// Gets all projects.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>List of projects.</returns>
    Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project by id.
    /// </summary>
    /// <param name="id">Project id.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Project or null.</returns>
    Task<ProjectDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="dto">Project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Created project.</returns>
    Task<ProjectDto> CreateAsync(ProjectDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="dto">Project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Updated project.</returns>
    Task<ProjectDto> UpdateAsync(ProjectDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a project by id.
    /// </summary>
    /// <param name="id">Project id.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if deleted.</returns>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
