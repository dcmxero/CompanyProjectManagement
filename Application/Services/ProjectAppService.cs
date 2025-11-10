using Application.Dtos;
using Domain.Models;
using Infrastructure.XmlStorage.Repositories;

namespace Application.Services;

/// <summary>
/// Default implementation of IProjectAppService.
/// </summary>
/// <remarks>
/// Initializes a new instance of the ProjectAppService class.
/// </remarks>
/// <param name="repo">XML project repository.</param>
public sealed class ProjectAppService(IXmlProjectRepository repo) : IProjectAppService
{
    private readonly IXmlProjectRepository _repo = repo;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<ProjectDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<ProjectDto> CreateAsync(ProjectDto dto, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(dto);
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            entity.Id = Guid.NewGuid().ToString("N");
        }
        var saved = await _repo.UpsertAsync(entity, cancellationToken);
        return MapToDto(saved);
    }

    /// <inheritdoc/>
    public async Task<ProjectDto> UpdateAsync(ProjectDto dto, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(dto);
        var saved = await _repo.UpsertAsync(entity, cancellationToken);
        return MapToDto(saved);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _repo.DeleteAsync(id, cancellationToken);
    }

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

    private static Project MapToEntity(ProjectDto d)
    {
        return new Project
        {
            Id = d.Id,
            Name = d.Name,
            Abbreviation = d.Abbreviation,
            Customer = d.Customer
        };
    }
}
