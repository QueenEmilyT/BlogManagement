using Asp.Versioning;
using Internship.Application.Authors.DTOs;
using Internship.Application.Authors.Services;
using Internship.Domain.Authors;
using Microsoft.AspNetCore.Mvc;

namespace Internship.API.Endpoints;

public static class AuthorsEndpoints
{
    public static IEndpointRouteBuilder MapAuthorsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi();
        var group = versionedApi.MapGroup("v{version:apiVersion}/authors")
                               .HasApiVersion(new ApiVersion(1, 0));

        // GET methods
        group.MapGet("/", async (
                [FromServices] AuthorApplicationService authorService) =>
            {
                var authors = await authorService.GetAllAuthorsAsync();
                return Results.Ok(authors);
            })
            .WithName("GetAllAuthors")
            .WithTags("Authors")
            .WithSummary("Get all authors with pagination")
            .WithDescription("Retrieves a paginated list of all authors")
            .Produces<IEnumerable<AuthorGridDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (
                [FromServices] AuthorApplicationService authorService,
                [FromRoute] Guid id) =>
            {
                var author = await authorService.GetAuthorByIdAsync(id);
                if (author == null)
                    return Results.NotFound();
                    
                return Results.Ok(author);
            })
            .WithName("GetAuthorById")
            .WithTags("Authors")
            .WithSummary("Get author by ID")
            .WithDescription("Retrieves a specific author by their unique identifier")
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/email/{email}", async (
                [FromServices] AuthorApplicationService authorService,
                [FromRoute] string email) =>
            {
                var author = await authorService.GetAuthorByEmailAsync(email);
                if (author == null)
                    return Results.NotFound();
                    
                return Results.Ok(author);
            })
            .WithName("GetAuthorByEmail")
            .WithTags("Authors")
            .WithSummary("Get author by email")
            .WithDescription("Retrieves a specific author by their email address")
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        

        group.MapGet("/search", async (
                [FromServices] AuthorApplicationService authorService,
                [FromQuery] string query,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10) =>
            {
                var authors = await authorService.SearchAuthorsAsync(query);
                return Results.Ok(authors);
            })
            .WithName("SearchAuthors")
            .WithTags("Authors")
            .WithSummary("Search for authors")
            .WithDescription("Searches for authors by name, email, or bio")
            .Produces<IEnumerable<AuthorGridDto>>(StatusCodes.Status200OK);

        // CREATE methods
        group.MapPost("/", async (
                [FromServices] AuthorApplicationService authorService,
                [FromBody] CreateAuthorDto createAuthorDto) =>
            {
                try
                {
                    var author = await authorService.CreateAuthorAsync(createAuthorDto);
                    return Results.Created($"/authors/{author.Id}", author);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("CreateAuthor")
            .WithTags("Authors")
            .WithSummary("Create a new author")
            .WithDescription("Creates a new author account")
            .Accepts<CreateAuthorDto>("application/json")
            .Produces<AuthorDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // UPDATE methods
        group.MapPut("/{id:guid}", async (
                [FromServices] AuthorApplicationService authorService,
                [FromRoute] Guid id,
                [FromBody] CreateAuthorDto updateAuthorDto) =>
            {
                try
                {
                    var author = await authorService.UpdateAuthorAsync(id, updateAuthorDto);
                    if (author == null)
                        return Results.NotFound();
                        
                    return Results.Ok(author);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("UpdateAuthor")
            .WithTags("Authors")
            .WithSummary("Update an author")
            .WithDescription("Updates an existing author's information")
            .Accepts<CreateAuthorDto>("application/json")
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        // DELETE methods
        group.MapDelete("/{id:guid}", async (
                [FromServices] AuthorApplicationService authorService,
                [FromRoute] Guid id) =>
            {
                var result = await authorService.DeleteAuthorAsync(id);
                if (!result)
                    return Results.NotFound();
                    
                return Results.NoContent();
            })
            .WithName("DeleteAuthor")
            .WithTags("Authors")
            .WithSummary("Delete an author")
            .WithDescription("Removes an author from the system")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // PROFILE methods
        group.MapPut("/{id:guid}/profile-picture", async (
                [FromServices] AuthorApplicationService authorService,
                [FromRoute] Guid id,
                [FromBody] string url) =>
            {
                var author = await authorService.UpdateProfilePictureAsync(id, url);
                if (author == null)
                    return Results.NotFound();
                    
                return Results.Ok(author);
            })
            .WithName("UpdateProfilePicture")
            .WithTags("Authors")
            .WithSummary("Update profile picture")
            .WithDescription("Updates an author's profile picture URL")
            .Accepts<string>("text/plain")
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/social-links", async (
                [FromServices] AuthorApplicationService authorService,
                [FromRoute] Guid id,
                [FromQuery] string twitter,
                [FromQuery] string facebook,
                [FromQuery] string linkedin,
                [FromQuery] string github) =>
            {
                var author = await authorService.UpdateSocialLinksAsync(id, twitter, facebook, linkedin, github);
                if (author == null)
                    return Results.NotFound();
                    
                return Results.Ok(author);
            })
            .WithName("UpdateSocialLinks")
            .WithTags("Authors")
            .WithSummary("Update social links")
            .WithDescription("Updates an author's social media links")
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
