using Application.Dtos;

namespace Application.Services;

/// <summary>
/// Provides project application operations.
/// </summary>
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
    /// Creates a new project. Returns handled result without throwing exceptions.
    /// </summary>
    /// <param name="dto">Project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>(Ok, Data, Error message).</returns>
    Task<(bool Ok, ProjectDto? Data, string? Error)> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project. Returns handled result without throwing exceptions.
    /// </summary>
    /// <param name="dto">Project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>(Ok, Data, Error message).</returns>
    Task<(bool Ok, ProjectDto? Data, string? Error)> UpdateAsync(ProjectDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a project by id. Returns handled result without throwing exceptions.
    /// </summary>
    /// <param name="id">Project id.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>(Ok, Error message).</returns>
    Task<(bool Ok, string? Error)> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
