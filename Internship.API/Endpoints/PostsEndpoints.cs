using Asp.Versioning;
using Internship.Application.Posts.DTOs;
using Internship.Application.Posts.Services;
using Microsoft.AspNetCore.Mvc;

namespace Internship.API.Endpoints;

public static class PostsEndpoints
{
    public static IEndpointRouteBuilder MapPostsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi();
        var group = versionedApi.MapGroup("v{version:apiVersion}/posts")
                               .HasApiVersion(new ApiVersion(1, 0));

        // GET methods
        group.MapGet("/", async (
                [FromServices] PostApplicationService postService) =>
            {
                var posts = await postService.GetAllPostsAsync();
                return Results.Ok(posts);
            })
            .WithName("GetAllPosts")
            .WithTags("Posts")
            .WithSummary("Get all posts with pagination")
            .WithDescription("Retrieves a paginated list of all blog posts")
            .Produces<IEnumerable<PostGridDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] Guid id) =>
            {
                var post = await postService.GetPostByIdAsync(id);
                if (post == null)
                    return Results.NotFound();
                    
                return Results.Ok(post);
            })
            .WithName("GetPostById")
            .WithTags("Posts")
            .WithSummary("Get post by ID")
            .WithDescription("Retrieves a specific blog post by its unique identifier")
            .Produces<PostDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/slug/{slug}", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] string slug) =>
            {
                var post = await postService.GetPostBySlugAsync(slug);
                if (post == null)
                    return Results.NotFound();
                    
                return Results.Ok(post);
            })
            .WithName("GetPostBySlug")
            .WithTags("Posts")
            .WithSummary("Get post by slug")
            .WithDescription("Retrieves a specific blog post by its URL-friendly slug")
            .Produces<PostDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/published", async (
                [FromServices] PostApplicationService postService, 
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10) =>
            {
                var posts = await postService.GetPublishedPostsAsync();
                return Results.Ok(posts);
            })
            .WithName("GetPublishedPosts")
            .WithTags("Posts")
            .WithSummary("Get published posts")
            .WithDescription("Retrieves a paginated list of all published blog posts")
            .Produces<IEnumerable<PostGridDto>>(StatusCodes.Status200OK);

        group.MapGet("/author/{authorId:guid}", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] Guid authorId,
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10) =>
            {
                var posts = await postService.GetPostsByAuthorIdAsync(authorId);
                return Results.Ok(posts);
            })
            .WithName("GetPostsByAuthor")
            .WithTags("Posts")
            .WithSummary("Get posts by author")
            .WithDescription("Retrieves a paginated list of posts by a specific author")
            .Produces<IEnumerable<PostGridDto>>(StatusCodes.Status200OK);

        // CREATE methods
        group.MapPost("/", async (
                [FromServices] PostApplicationService postService, 
                [FromBody] CreatePostDto createPostDto) =>
            {
                try
                {
                    var post = await postService.CreatePostAsync(createPostDto);
                    return Results.Created($"/posts/{post.Id}", post);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("CreatePost")
            .WithTags("Posts")
            .WithSummary("Create a new post")
            .WithDescription("Creates a new blog post")
            .Accepts<CreatePostDto>("application/json")
            .Produces<PostDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // UPDATE methods
        group.MapPut("/{id:guid}", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] Guid id, 
                [FromBody] CreatePostDto updatePostDto) =>
            {
                try
                {
                    var post = await postService.UpdatePostAsync(id, updatePostDto);
                    if (post == null)
                        return Results.NotFound();
                        
                    return Results.Ok(post);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("UpdatePost")
            .WithTags("Posts")
            .WithSummary("Update a post")
            .WithDescription("Updates an existing blog post")
            .Accepts<CreatePostDto>("application/json")
            .Produces<PostDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // DELETE methods
        group.MapDelete("/{id:guid}", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] Guid id) =>
            {
                var result = await postService.DeletePostAsync(id);
                if (!result)
                    return Results.NotFound();
                    
                return Results.NoContent();
            })
            .WithName("DeletePost")
            .WithTags("Posts")
            .WithSummary("Delete a post")
            .WithDescription("Deletes a blog post")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // PUBLISH/ARCHIVE methods
        group.MapPost("/{id:guid}/publish", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] Guid id) =>
            {
                var post = await postService.PublishPostAsync(id);
                if (post == null)
                    return Results.NotFound();
                    
                return Results.Ok(post);
            })
            .WithName("PublishPost")
            .WithTags("Posts")
            .WithSummary("Publish a post")
            .WithDescription("Changes a post's status to published")
            .Produces<PostDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/archive", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] Guid id) =>
            {
                var post = await postService.ArchivePostAsync(id);
                if (post == null)
                    return Results.NotFound();
                    
                return Results.Ok(post);
            })
            .WithName("ArchivePost")
            .WithTags("Posts")
            .WithSummary("Archive a post")
            .WithDescription("Archives a blog post, making it unavailable to readers")
            .Produces<PostDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/restore", async (
                [FromServices] PostApplicationService postService, 
                [FromRoute] Guid id) =>
            {
                var post = await postService.RestorePostAsync(id);
                if (post == null)
                    return Results.NotFound();
                    
                return Results.Ok(post);
            })
            .WithName("RestorePost")
            .WithTags("Posts")
            .WithSummary("Restore a post")
            .WithDescription("Restores an archived post")
            .Produces<PostDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);


        return app;
    }
}