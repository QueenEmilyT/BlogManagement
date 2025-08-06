using Internship.Blazor.Exceptions;
using Internship.Blazor.Pages.Posts.DTOs;
using Internship.Blazor.Pages.Posts.Operations.Creation;
using Internship.Blazor.Pages.Posts.Operations.Edition;
using Internship.Blazor.Pages.Posts.Services;
using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Pages.Posts;

public partial class Posts(PostsService service, ILogger<Posts> logger)
{
    [Parameter] public Guid? AuthorId { get; set; }

    private PostCreator _postCreator;
    private PostEditor _postEditor;
    private bool ShowConfigDropdown { get; set; } = false;
    
    private IEnumerable<PostGridDto> _selectedPosts { get; set; } = new List<PostGridDto>();
    
    private List<PostGridDto> _posts { get; set; } = [];
    private PostGridDto? _selectedPost { get; set; }
    
    [Inject] protected NavigationManager NavigationManager { get; set; }
    
    
    // The signature needs to be 'object' rather than void
    private void CreatePost(object obj) => _postCreator.Show();
    
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    public async Task Refresh()
    {
        await LoadData();
    }
    
    private async Task LoadData()
    {
        try
        {
            if (AuthorId.HasValue)
            {
                _posts = await service.GetPostsByAuthorAsync(AuthorId.Value);
            }
            else
            {
                _posts = await service.GetPostsAsync();
            }
            StateHasChanged();
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }

    private async Task DeletePost(PostGridDto post)
    {
        try
        {
            await service.DeletePostAsync(post.Id);
            await Refresh(); 
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }
    
    // Handle row clicks with selection toggle capability
    private void RowClicked(PostGridDto post)
    {
        var currentList = _selectedPosts.ToList();
        
        // If the item is already selected, deselect it (toggle behavior)
        if (currentList.Contains(post))
        {
            currentList.Remove(post);
            _selectedPost = null;
        }
        else
        {
            
            _selectedPost = post;
            currentList.Add(post);
        }
        
        _selectedPosts = currentList;
        StateHasChanged(); 
    }

    private void RowDoubleClicked(PostGridDto post)
    {
        _selectedPost = post;
        _selectedPosts = new List<PostGridDto> { post };
        
        // Navigate to the application details page
        EditPost(post);
    }

    private void SelectedApplicationsChanged(IEnumerable<PostGridDto> posts)
    {
        _selectedPosts = posts;
        _selectedPost = posts.FirstOrDefault();
        StateHasChanged(); 
    }

    private async Task EditPost(PostGridDto post)
    {
        await _postEditor.ShowAsync(post.Id);
    }
    
    private void ExportToExcel()
    {
        ShowMessage("Export functionality not yet implemented.");
    }
    
    private void NavigateToConfiguration(string section)
    {
        ShowConfigDropdown = false;
        NavigationManager.NavigateTo(section);
    }

    
    
    private void ShowMessage(string message)
    {
        // Implementation depends on your BaseComponent or notification system
    }
}