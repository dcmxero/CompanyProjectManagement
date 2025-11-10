using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Exposes CRUD endpoints for projects.
/// </summary>
/// <remarks>
/// Initializes a new instance of the ProjectsController class.
/// </remarks>
/// <param name="service">Project application service.</param>
[ApiController]
[Route("api/[controller]")]
public sealed class ProjectsController(IProjectAppService service) : ControllerBase
{
    private readonly IProjectAppService _service = service;

    /// <summary>
    /// Gets all projects.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Projects.</returns>
    [HttpGet]
    public Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => _service.GetAllAsync(cancellationToken);

    /// <summary>
    /// Gets a project by id.
    /// </summary>
    /// <param name="id">Project id.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Project or 404.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }
        return item;
    }

    /// <summary>
    /// Creates a project.
    /// </summary>
    /// <param name="dto">Project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Created project.</returns>
    [HttpPost]
    public Task<ProjectDto> CreateAsync([FromBody] ProjectDto dto, CancellationToken cancellationToken = default)
        => _service.CreateAsync(dto, cancellationToken);

    /// <summary>
    /// Updates a project.
    /// </summary>
    /// <param name="dto">Project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Updated project.</returns>
    [HttpPut]
    public Task<ProjectDto> UpdateAsync([FromBody] ProjectDto dto, CancellationToken cancellationToken = default)
        => _service.UpdateAsync(dto, cancellationToken);

    /// <summary>
    /// Deletes a project by id.
    /// </summary>
    /// <param name="id">Project id.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var ok = await _service.DeleteAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
