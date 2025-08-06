using Asp.Versioning;
using Internship.Application.Tags.DTOs;
using Internship.Application.Tags.Services;
using Microsoft.AspNetCore.Mvc;

namespace Internship.API.Endpoints;

public static class TagsEndpoints
{
    public static IEndpointRouteBuilder MapTagsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi();
        var group = versionedApi.MapGroup("v{version:apiVersion}/tags")
                               .HasApiVersion(new ApiVersion(1, 0));

        // GET methods
        group.MapGet("/", async (
                [FromServices] TagApplicationService tagService) =>
            {
                var tags = await tagService.GetAllTagsAsync();
                return Results.Ok(tags);
            })
            .WithName("GetAllTags")
            .WithTags("Tags")
            .WithSummary("Get all tags")
            .WithDescription("Retrieves a list of all tags")
            .Produces<IEnumerable<TagGridDto>>(StatusCodes.Status200OK);

        

        group.MapGet("/{id:guid}", async (
                [FromServices] TagApplicationService tagService, 
                [FromRoute] Guid id) =>
            {
                var tag = await tagService.GetTagByIdAsync(id);
                if (tag == null)
                    return Results.NotFound();
                    
                return Results.Ok(tag);
            })
            .WithName("GetTagById")
            .WithTags("Tags")
            .WithSummary("Get tag by ID")
            .WithDescription("Retrieves a specific tag by its unique identifier")
            .Produces<TagDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/name/{name}", async (
                [FromServices] TagApplicationService tagService, 
                [FromRoute] string name) =>
            {
                var tag = await tagService.GetTagByNameAsync(name);
                if (tag == null)
                    return Results.NotFound();
                    
                return Results.Ok(tag);
            })
            .WithName("GetTagByName")
            .WithTags("Tags")
            .WithSummary("Get tag by name")
            .WithDescription("Retrieves a specific tag by its name")
            .Produces<TagDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/post/{postId:guid}", async (
                [FromServices] TagApplicationService tagService, 
                [FromRoute] Guid postId) =>
            {
                var tags = await tagService.GetTagsByPostIdAsync(postId);
                return Results.Ok(tags);
            })
            .WithName("GetTagsByPost")
            .WithTags("Tags")
            .WithSummary("Get tags by post")
            .WithDescription("Retrieves all tags associated with a specific post")
            .Produces<IEnumerable<TagDto>>(StatusCodes.Status200OK);

        // CREATE methods
        group.MapPost("/", async (
                [FromServices] TagApplicationService tagService, 
                [FromBody] CreateTagDto createTagDto) =>
            {
                try
                {
                    var tag = await tagService.CreateTagAsync(createTagDto);
                    return Results.Created($"/tags/{tag.Id}", tag);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("CreateTag")
            .WithTags("Tags")
            .WithSummary("Create a new tag")
            .WithDescription("Creates a new tag")
            .Accepts<CreateTagDto>("application/json")
            .Produces<TagDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // UPDATE methods
        group.MapPut("/{id:guid}", async (
                [FromServices] TagApplicationService tagService, 
                [FromRoute] Guid id, 
                [FromBody] CreateTagDto updateTagDto) =>
            {
                try
                {
                    var tag = await tagService.UpdateTagAsync(id, updateTagDto);
                    if (tag == null)
                        return Results.NotFound();
                        
                    return Results.Ok(tag);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("UpdateTag")
            .WithTags("Tags")
            .WithSummary("Update a tag")
            .WithDescription("Updates an existing tag")
            .Accepts<CreateTagDto>("application/json")
            .Produces<TagDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // DELETE methods
        group.MapDelete("/{id:guid}", async (
                [FromServices] TagApplicationService tagService, 
                [FromRoute] Guid id) =>
            {
                var result = await tagService.DeleteTagAsync(id);
                if (!result)
                    return Results.NotFound();
                    
                return Results.NoContent();
            })
            .WithName("DeleteTag")
            .WithTags("Tags")
            .WithSummary("Delete a tag")
            .WithDescription("Deletes an existing tag")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // Tag-Post association methods
        group.MapPost("/{tagId:guid}/posts/{postId:guid}", async (
                [FromServices] TagApplicationService tagService, 
                [FromRoute] Guid tagId, 
                [FromRoute] Guid postId) =>
            {
                try
                {
                    await tagService.AddTagToPostAsync(tagId, postId);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("AddTagToPost")
            .WithTags("Tags")
            .WithSummary("Add tag to post")
            .WithDescription("Associates a tag with a post")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{tagId:guid}/posts/{postId:guid}", async (
                [FromServices] TagApplicationService tagService, 
                [FromRoute] Guid tagId, 
                [FromRoute] Guid postId) =>
            {
                try
                {
                    await tagService.RemoveTagFromPostAsync(tagId, postId);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("RemoveTagFromPost")
            .WithTags("Tags")
            .WithSummary("Remove tag from post")
            .WithDescription("Removes a tag association from a post")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}