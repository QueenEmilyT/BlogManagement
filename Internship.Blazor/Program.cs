using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Internship.Blazor;
using Internship.Blazor.Pages.Authors;
using Internship.Blazor.Pages.Comments;
using Internship.Blazor.Pages.Posts.Services;
using Internship.Blazor.Pages.Tags;
using Internship.Blazor.Services;
using Polly;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddTelerikBlazor();
builder.Services.AddScoped<PostsService>();
builder.Services.AddScoped<CommentsService>();
builder.Services.AddScoped<AuthorsService>();
builder.Services.AddScoped<TagsService>();
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5065/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30); 
}).AddTransientHttpErrorPolicy(policyBuilder => policyBuilder
    .OrResult(response => 
    {
        var statusCode = (int)response.StatusCode;
        return statusCode >= 500 && statusCode != 503; 
    })
    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(1 * retryAttempt)));

await builder.Build().RunAsync();