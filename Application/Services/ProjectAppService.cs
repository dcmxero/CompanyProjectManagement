using Application.Dtos;
using Domain.Models;
using Infrastructure.XmlStorage.Repositories;

namespace Application.Services;

public sealed class ProjectAppService(IXmlProjectRepository repo) : IProjectAppService
{
    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await repo.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList().AsReadOnly();
    }

    public async Task<ProjectDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await repo.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<(bool Ok, ProjectDto? Data, string? Error)> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var err = Validate(dto);
        if (err is not null)
        {
            return (false, null, err);
        }

        var allProjects = await repo.GetAllAsync(cancellationToken);
        var nextId = GenerateNextId(allProjects.Select(x => x.Id));

        var entity = MapToEntity(dto);
        entity.Id = nextId;

        var saved = await repo.UpsertAsync(entity, cancellationToken);
        return (true, MapToDto(saved), null);
    }

    public async Task<(bool Ok, ProjectDto? Data, string? Error)> UpdateAsync(ProjectDto dto, CancellationToken cancellationToken = default)
    {
        var err = ValidateUpdate(dto);
        if (err is not null)
        {
            return (false, null, err);
        }

        var exists = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (exists is null)
        {
            return (false, null, $"Project with id '{dto.Id}' was not found.");
        }

        var saved = await repo.UpsertAsync(MapUpdateToEntity(dto), cancellationToken);
        return (true, MapToDto(saved), null);
    }

    public async Task<(bool Ok, string? Error)> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return (false, "Project ID must not be empty.");
        }

        var existed = await repo.DeleteAsync(id, cancellationToken);
        if (!existed)
        {
            return (false, $"Project with id '{id}' was not found.");
        }
        return (true, null);
    }

    #region private methods

    /// <summary>
    /// Validates a new project request and returns a validation error message if invalid.
    /// </summary>
    /// <param name="d">CreateProjectDto containing data for the new project.</param>
    /// <returns>Validation error message if invalid; otherwise null.</returns>
    private static string? Validate(CreateProjectDto d)
    {
        if (string.IsNullOrWhiteSpace(d.Name))
        {
            return "Project name must not be empty.";
        }
        if (string.IsNullOrWhiteSpace(d.Abbreviation))
        {
            return "Project abbreviation must not be empty.";
        }
        if (string.IsNullOrWhiteSpace(d.Customer))
        {
            return "Project customer must not be empty.";
        }
        return null;
    }

    /// <summary>
    /// Validates an update request for an existing project and returns a validation error message if invalid.
    /// </summary>
    /// <param name="d">ProjectDto containing data to update the project.</param>
    /// <returns>Validation error message if invalid; otherwise null.</returns>
    private static string? ValidateUpdate(ProjectDto d)
    {
        if (string.IsNullOrWhiteSpace(d.Id))
        {
            return "Project Id must not be empty.";
        }
        if (string.IsNullOrWhiteSpace(d.Name))
        {
            return "Project name must not be empty.";
        }
        if (string.IsNullOrWhiteSpace(d.Abbreviation))
        {
            return "Project abbreviation must not be empty.";
        }
        if (string.IsNullOrWhiteSpace(d.Customer))
        {
            return "Project customer must not be empty.";
        }
        return null;
    }

    /// <summary>
    /// Maps a Project entity to a ProjectDto.
    /// </summary>
    /// <param name="e">Project entity to map from.</param>
    /// <returns>Mapped ProjectDto.</returns>
    private static ProjectDto MapToDto(Project e)
    {
        return new ProjectDto
        {
            Id = e.Id,
            Name = e.Name,
            Abbreviation = e.Abbreviation,
            Customer = e.Customer
        };
    }

    /// <summary>
    /// Maps a CreateProjectDto to a new Project entity.
    /// </summary>
    /// <param name="d">CreateProjectDto to map from.</param>
    /// <returns>New Project entity.</returns>
    private static Project MapToEntity(CreateProjectDto d)
    {
        return new Project
        {
            Name = d.Name,
            Abbreviation = d.Abbreviation,
            Customer = d.Customer
        };
    }

    /// <summary>
    /// Maps a ProjectDto to a Project entity for updates.
    /// </summary>
    /// <param name="d">ProjectDto to map from.</param>
    /// <returns>Project entity populated from the DTO.</returns>
    private static Project MapUpdateToEntity(ProjectDto d)
    {
        return new Project
        {
            Id = d.Id,
            Name = d.Name,
            Abbreviation = d.Abbreviation,
            Customer = d.Customer
        };
    }

    /// <summary>
    /// Generates the next project identifier in the format 'prj' + (max numeric suffix + 1).
    /// </summary>
    /// <param name="existingIds">Collection of existing project identifiers.</param>
    /// <returns>New project identifier.</returns>
    private static string GenerateNextId(IEnumerable<string> existingIds)
    {
        var max = 0;
        foreach (var id in existingIds)
        {
            if (id?.StartsWith("prj", StringComparison.OrdinalIgnoreCase) == true &&
                int.TryParse(id[3..], out var num) && num > max)
            {
                max = num;
            }
        }
        return $"prj{max + 1}";
    }

    #endregion
}
