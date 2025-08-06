using Internship.Blazor.Exceptions;
using Internship.Blazor.Pages.Comments.DTOs;
using Internship.Blazor.Pages.Comments.Operations.Creation;
using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Pages.Comments;

public partial class Comments(CommentsService service, ILogger<Comments> logger)
{
    private CommentCreator _commentCreator;
    
    [Parameter] public Guid postId { get; set; }
    
    private IEnumerable<CommentGridDto> _selectedComments { get; set; } = [];
    
    private List<CommentGridDto> _comments { get; set; } = [];
    private CommentGridDto? _selectedComment { get; set; }
    
    [Inject] protected NavigationManager NavigationManager { get; set; }
    
    
    // The signature needs to be 'object' rather than void
    private void CreateComment(object obj) => _commentCreator.Show();
    
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
            _comments = await service.GetCommentsAsync(postId);
            StateHasChanged();
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }

    private async Task DeleteComment(CommentGridDto comment)
    {
        try
        {
            await service.DeleteCommentAsync(comment.Id);
            await Refresh(); 
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }
    
    // Handle row clicks with selection toggle capability
    private void RowClicked(CommentGridDto comment)
    {
        var currentList = _selectedComments.ToList();
        if (currentList.Contains(comment))
        {
            currentList.Remove(comment);
            _selectedComment = null;
        }
        else
        {
            _selectedComment = comment;
            currentList.Add(comment);
        }
        
        _selectedComments = currentList;
        StateHasChanged(); 
    }

    private void RowDoubleClicked(CommentGridDto comment)
    {
        _selectedComment = comment;
        _selectedComments = new List<CommentGridDto> { comment };
        
        EditComment(comment);
    }

    // Handle selection changes from the grid
    private void SelectedCommentsChanged(IEnumerable<CommentGridDto> comments)
    {
        _selectedComments = comments;
        _selectedComment = comments.FirstOrDefault();
        StateHasChanged(); 
    }

    private void EditComment(CommentGridDto comment)
    {
        NavigationManager.NavigateTo($"/posts/{comment.Id}/edit");
    }
    
    private void ShowMessage(string message)
    {
        // Implementation depends on your BaseComponent or notification system
    }
}