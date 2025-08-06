using Internship.Blazor.Exceptions;
using Internship.Blazor.Pages.Authors.DTOs;
using Internship.Blazor.Pages.Authors.Operations.Creation;
using Internship.Blazor.Pages.Authors.Operations.Edition;
using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Pages.Authors;

public partial class Authors(AuthorsService service, ILogger<Authors> logger)
{

    private AuthorCreator _authorCreator;
    private AuthorEditor _authorEditor;
    private bool ShowConfigDropdown { get; set; } = false;
    
    private IEnumerable<AuthorGridDto> _selectedAuthors { get; set; } = [];
    
    private List<AuthorGridDto> _authors { get; set; } = [];
    private AuthorGridDto? _selectedAuthor { get; set; }
    
    [Inject] protected NavigationManager NavigationManager { get; set; }
    
    
    private void CreateAuthor(object obj) => _authorCreator.Show();
    
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
            _authors = await service.GetAuthorsAsync();
            StateHasChanged();
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }

    private async Task DeleteAuthor(AuthorGridDto author)
    {
        try
        {
            await service.DeleteAuthorAsync(author.Id);
            await Refresh(); 
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }
    
    private void RowClicked(AuthorGridDto author)
    {
        var currentList = _selectedAuthors.ToList();
        
        if (currentList.Contains(author))
        {
            currentList.Remove(author);
            _selectedAuthor = null;
        }
        else
        {
            _selectedAuthor = author;
            currentList.Add(author);
        }
        
        _selectedAuthors = currentList;
        StateHasChanged(); 
    }

    private void RowDoubleClicked(AuthorGridDto author)
    {
        _selectedAuthor = author;
        _selectedAuthors = new List<AuthorGridDto> { author };
        
        EditAuthor(author);
    }

    private void SelectedAuthorsChanged(IEnumerable<AuthorGridDto> authors)
    {
        _selectedAuthors = authors;
        _selectedAuthor = authors.FirstOrDefault();
        StateHasChanged(); 
    }

    private async Task EditAuthor(AuthorGridDto author)
    {
        await _authorEditor.ShowAsync(author.Id);
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
    }
}