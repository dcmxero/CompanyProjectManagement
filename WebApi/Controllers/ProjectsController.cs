using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Provides endpoints for managing company projects.
/// </summary>
/// <remarks>
/// This controller allows clients to perform CRUD operations on projects,
/// including listing, retrieving, creating, updating, and deleting project records.
/// </remarks>
[ApiController]
[Route("api/projects")]
[Produces("application/json")]
[Tags("Projects")]
public sealed class ProjectsController(
    IProjectAppService service,
    ILogger<ProjectsController> log) : ControllerBase
{
    /// <summary>
    /// Retrieves all existing projects.
    /// </summary>
    /// <remarks>
    /// Returns a list of all projects currently stored in the XML repository.
    /// </remarks>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A read-only list of all <see cref="ProjectDto"/> objects.</returns>
    [HttpGet("GetProjects")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ProjectDto>))]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        log.LogInformation("GET GetProjects: start");
        try
        {
            var items = await service.GetAllAsync(cancellationToken);
            log.LogInformation("GET GetProjects: ok, count={Count}", items.Count);
            return Ok(items);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "GET GetProjects: unhandled error");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific project by its unique identifier.
    /// </summary>
    /// <param name="id">Unique project identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The requested <see cref="ProjectDto"/> object if found; otherwise, a 404 response.</returns>
    [HttpGet("{id}", Name = "GetProjectById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetByIdAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        log.LogInformation("GET GetById: id={ProjectId}", id);
        try
        {
            var item = await service.GetByIdAsync(id, cancellationToken);
            if (item == null)
            {
                log.LogWarning("GET GetById: not found, id={ProjectId}", id);
                return NotFound(new { error = $"Project with id '{id}' was not found." });
            }

            log.LogInformation("GET GetById: ok, id={ProjectId}", id);
            return Ok(item);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "GET GetById: unhandled error, id={ProjectId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new project record.
    /// </summary>
    /// <remarks>
    /// Requires a valid Bearer token.  
    /// The created project will be stored in the XML repository.
    /// </remarks>
    /// <param name="dto">The data used to create the project.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The created <see cref="ProjectDto"/> object if successful.</returns>
    [HttpPost("CreateProject")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> CreateAsync([FromBody] CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        log.LogInformation("POST CreateProject: payload={@Payload}", dto);
        try
        {
            var (ok, data, error) = await service.CreateAsync(dto, cancellationToken);

            if (!ok)
            {
                log.LogWarning("POST CreateProject: bad request, error={Error}", error);
                return BadRequest(new { error });
            }

            log.LogInformation("POST CreateProject: created id={ProjectId}", data!.Id);
            return CreatedAtRoute("GetProjectById", new { id = data!.Id }, data);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "POST CreateProject: unhandled error");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing project record.
    /// </summary>
    /// <remarks>
    /// Requires a valid Bearer token.  
    /// Updates the fields of an existing project based on the provided data.
    /// </remarks>
    /// <param name="dto">The updated project data.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The updated <see cref="ProjectDto"/> object if successful.</returns>
    [HttpPut("UpdateProject")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> UpdateAsync([FromBody] ProjectDto dto, CancellationToken cancellationToken = default)
    {
        log.LogInformation("PUT UpdateProject: id={ProjectId}, payload={@Payload}", dto.Id, dto);
        try
        {
            var (ok, data, error) = await service.UpdateAsync(dto, cancellationToken);

            if (!ok)
            {
                log.LogWarning("PUT UpdateProject: bad request, id={ProjectId}, error={Error}", dto.Id, error);
                return BadRequest(new { error });
            }

            log.LogInformation("PUT UpdateProject: ok, id={ProjectId}", data!.Id);
            return Ok(data);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "PUT UpdateProject: unhandled error, id={ProjectId}", dto.Id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a project by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Requires a valid Bearer token.  
    /// If the specified project does not exist, returns 404 Not Found.
    /// </remarks>
    /// <param name="id">Unique project identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A 204 response if the deletion was successful.</returns>
    [HttpDelete("{id}", Name = "DeleteProject")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        log.LogInformation("DELETE DeleteProject: id={ProjectId}", id);
        try
        {
            var (ok, error) = await service.DeleteAsync(id, cancellationToken);

            if (!ok)
            {
                if (!string.IsNullOrWhiteSpace(error) &&
                    error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    log.LogWarning("DELETE DeleteProject: not found, id={ProjectId}", id);
                    return NotFound(new { error });
                }

                log.LogWarning("DELETE DeleteProject: bad request, id={ProjectId}, error={Error}", id, error);
                return BadRequest(new { error });
            }

            log.LogInformation("DELETE DeleteProject: ok, id={ProjectId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            log.LogError(ex, "DELETE DeleteProject: unhandled error, id={ProjectId}", id);
            throw;
        }
    }
}
