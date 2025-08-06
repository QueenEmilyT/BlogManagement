using Internship.Application.Tags.DTOs;
using Internship.Domain.Tags;

namespace Internship.Application.Tags.Services;

public class TagApplicationService(ITagRepository tagRepository)
{
    public async Task<TagDto?> GetTagByIdAsync(Guid id)
    {
        var tag = await tagRepository.GetByIdAsync(id);
        return tag != null ? MapToTagDto(tag) : null;
    }

    public async Task<TagDto?> GetTagByNameAsync(string name)
    {
        var tag = await tagRepository.GetByNameAsync(name);
        return tag != null ? MapToTagDto(tag) : null;
    }

    public async Task<IEnumerable<TagGridDto>> GetAllTagsAsync()
    {
        var tags = await tagRepository.GetAllAsync();
        return tags.Select(MapToTagGridDto);
    }

    public async Task<IEnumerable<TagGridDto>> GetAllTagsForGridAsync()
    {
        var tags = await tagRepository.GetAllAsync();
        return tags.Select(MapToTagGridDto);
    }

    public async Task<TagDto> CreateTagAsync(CreateTagDto createTagDto)
    {
        var tag = new Tag(Guid.NewGuid(), createTagDto.Name, createTagDto.Description);
        var createdTag = await tagRepository.CreateAsync(tag);
        return MapToTagDto(createdTag);
    }

    public async Task<TagDto?> UpdateTagAsync(Guid id, CreateTagDto updateTagDto)
    {
        var tag = await tagRepository.GetByIdAsync(id);
        if (tag == null)
            return null;

        tag.Update(updateTagDto.Name, updateTagDto.Description);
        var updatedTag = await tagRepository.UpdateAsync(tag);
        return MapToTagDto(updatedTag);
    }

    public async Task<bool> DeleteTagAsync(Guid id)
    {
        return await tagRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TagDto>> GetTagsByPostIdAsync(Guid postId)
    {
        var tags = await tagRepository.GetTagsByPostIdAsync(postId);
        return tags.Select(MapToTagDto);
    }

    public async Task AddTagToPostAsync(Guid tagId, Guid postId)
    {
        await tagRepository.AddTagToPostAsync(tagId, postId);
    }

    public async Task RemoveTagFromPostAsync(Guid tagId, Guid postId)
    {
        await tagRepository.RemoveTagFromPostAsync(tagId, postId);
    }

    private TagDto MapToTagDto(Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            CreatedAt = tag.CreatedAt
        };
    }

    private TagGridDto MapToTagGridDto(Tag tag)
    {
        return new TagGridDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            CreatedAt = tag.CreatedAt
        };
    }
    
}