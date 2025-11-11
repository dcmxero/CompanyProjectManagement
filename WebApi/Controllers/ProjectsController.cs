using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Provides endpoints for managing company projects.
/// </summary>
[ApiController]
[Route("api/projects")]
[Produces("application/json")]
[Tags("Projects")]
public sealed class ProjectsController(IProjectAppService service) : ControllerBase
{
    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    [HttpGet("GetProjects")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ProjectDto>))]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await service.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// </summary>
    [HttpGet("{id}", Name = "GetProjectById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetByIdAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        var item = await service.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            return NotFound(new { error = $"Project with id '{id}' was not found." });
        }

        return Ok(item);
    }

    /// <summary>
    /// Creates a new project. Requires Bearer authorization.
    /// </summary>
    [HttpPost("CreateProject")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> CreateAsync([FromBody] CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var (ok, data, error) = await service.CreateAsync(dto, cancellationToken);

        if (!ok)
        {
            return BadRequest(new { error });
        }

        return CreatedAtRoute("GetProjectById", new { id = data!.Id }, data);
    }

    /// <summary>
    /// Updates an existing project. Requires Bearer authorization.
    /// </summary>
    [HttpPut("UpdateProject")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> UpdateAsync([FromBody] ProjectDto dto, CancellationToken cancellationToken = default)
    {
        var (ok, data, error) = await service.UpdateAsync(dto, cancellationToken);

        if (!ok)
        {
            return BadRequest(new { error });
        }

        return Ok(data);
    }

    /// <summary>
    /// Deletes a project by id. Requires Bearer authorization.
    /// </summary>
    [HttpDelete("{id}", Name = "DeleteProject")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        var (ok, error) = await service.DeleteAsync(id, cancellationToken);

        if (!ok)
        {
            if (error != null && error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new { error });
            }

            return BadRequest(new { error });
        }

        return NoContent();
    }
}
