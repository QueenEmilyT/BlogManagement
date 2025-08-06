using Asp.Versioning;
using Internship.Application.Comments.DTOs;
using Internship.Application.Comments.Services;
using Microsoft.AspNetCore.Mvc;

namespace Internship.API.Endpoints;

public static class CommentsEndpoints
{
    public static IEndpointRouteBuilder MapCommentsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi();
        var group = versionedApi.MapGroup("v{version:apiVersion}/comments")
                               .HasApiVersion(new ApiVersion(1, 0));

        // GET methods
        group.MapGet("/{id:guid}", async (
                [FromServices] CommentApplicationService commentService,
                [FromRoute] Guid id) =>
            {
                var comment = await commentService.GetCommentByIdAsync(id);
                if (comment == null)
                    return Results.NotFound();

                return Results.Ok(comment);
            })
            .WithName("GetCommentById")
            .WithTags("Comments")
            .WithSummary("Get comment by ID")
            .WithDescription("Retrieves a specific comment by its unique identifier")
            .Produces<CommentDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/post/{postId:guid}", async (
                [FromServices] CommentApplicationService commentService,
                [FromRoute] Guid postId,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10) =>
            {
                var comments = await commentService.GetCommentsByPostIdAsync(postId);
                return Results.Ok(comments);
            })
            .WithName("GetCommentsByPostId")
            .WithTags("Comments")
            .WithSummary("Get comments by post ID")
            .WithDescription("Retrieves all comments for a specific blog post")
            .Produces<IEnumerable<CommentGridDto>>(StatusCodes.Status200OK);

        

        group.MapGet("/author/{authorId:guid}", async (
                [FromServices] CommentApplicationService commentService,
                [FromRoute] Guid authorId,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10) =>
            {
                var comments = await commentService.GetCommentsByAuthorIdAsync(authorId);
                return Results.Ok(comments);
            })
            .WithName("GetCommentsByAuthorId")
            .WithTags("Comments")
            .WithSummary("Get comments by author ID")
            .WithDescription("Retrieves all comments made by a specific author")
            .Produces<IEnumerable<CommentGridDto>>(StatusCodes.Status200OK);

        group.MapGet("/recent", async (
                [FromServices] CommentApplicationService commentService,
                [FromQuery] int count = 10) =>
            {
                var comments = await commentService.GetRecentCommentsAsync(count);
                return Results.Ok(comments);
            })
            .WithName("GetRecentComments")
            .WithTags("Comments")
            .WithSummary("Get recent comments")
            .WithDescription("Retrieves the most recently posted comments")
            .Produces<IEnumerable<CommentGridDto>>(StatusCodes.Status200OK);

        group.MapGet("/{commentId:guid}/replies", async (
                [FromServices] CommentApplicationService commentService,
                [FromRoute] Guid commentId) =>
            {
                var replies = await commentService.GetRepliesAsync(commentId);
                return Results.Ok(replies);
            })
            .WithName("GetCommentReplies")
            .WithTags("Comments")
            .WithSummary("Get comment replies")
            .WithDescription("Retrieves all replies to a specific comment")
            .Produces<IEnumerable<CommentDto>>(StatusCodes.Status200OK);

       

        // CREATE methods
        group.MapPost("/", async (
                [FromServices] CommentApplicationService commentService,
                [FromBody] CreateCommentDto createCommentDto) =>
            {
                try
                {
                    var comment = await commentService.CreateCommentAsync(createCommentDto);
                    return Results.Created($"/comments/{comment.Id}", comment);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("CreateComment")
            .WithTags("Comments")
            .WithSummary("Create a new comment")
            .WithDescription("Posts a new comment or reply to a blog post")
            .Accepts<CreateCommentDto>("application/json")
            .Produces<CommentDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // UPDATE methods
        group.MapPut("/{id:guid}", async (
                [FromServices] CommentApplicationService commentService,
                [FromRoute] Guid id,
                [FromBody] string content) =>
            {
                try
                {
                    var comment = await commentService.UpdateCommentAsync(id, content);
                    if (comment == null)
                        return Results.NotFound();

                    return Results.Ok(comment);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("UpdateComment")
            .WithTags("Comments")
            .WithSummary("Update a comment")
            .WithDescription("Edits the content of an existing comment")
            .Accepts<string>("text/plain")
            .Produces<CommentDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // DELETE methods
        group.MapDelete("/{id:guid}", async (
                [FromServices] CommentApplicationService commentService,
                [FromRoute] Guid id) =>
            {
                var result = await commentService.DeleteCommentAsync(id);
                if (!result)
                    return Results.NotFound();

                return Results.NoContent();
            })
            .WithName("DeleteComment")
            .WithTags("Comments")
            .WithSummary("Delete a comment")
            .WithDescription("Removes a comment from the system")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);


        return app;
    }
}
