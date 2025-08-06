using Asp.Versioning;
using Internship.API.Endpoints;
using Internship.Application.Authors.Services;
using Internship.Application.Comments.Services;
using Internship.Application.Posts.Services;
using Internship.Application.Tags.Services;
using Internship.Domain.Authors;
using Internship.Domain.Comments;
using Internship.Domain.Posts;
using Internship.Domain.Tags;
using Internship.Infrastructure;
using Internship.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Internship.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();

        builder.Services.AddOpenApi();

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("BlazorAppPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Add API versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "Invoice Management API",
                Version = "v1",
                Description = "API for managing posts, comments,and authors"
            });
        });

        // Register DbContext
        builder.Services.AddDbContext<InternContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();

        // Register application services
        builder.Services.AddScoped<PostApplicationService>();
        builder.Services.AddScoped<CommentApplicationService>();
        builder.Services.AddScoped<AuthorApplicationService>();
        builder.Services.AddScoped<TagApplicationService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice Management API v1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseCors("BlazorAppPolicy");
        // Map endpoints
        app.MapPostsEndpoints();
        app.MapCommentsEndpoints();
        app.MapAuthorsEndpoints();
        app.MapTagsEndpoints();

        app.Run();
    }
}