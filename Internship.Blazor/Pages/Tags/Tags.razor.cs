using Internship.Blazor.Exceptions;
using Internship.Blazor.Pages.Tags.DTOs;
using Internship.Blazor.Pages.Tags.Operations.Creation;
using Internship.Blazor.Pages.Tags.Operations.Edition;
using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Pages.Tags;

public partial class Tags(TagsService service)
{
    private TagCreator _tagCreator;
    private TagEditor _tagEditor;
    
    [Parameter] public Guid? PostId { get; set; }
    
    private IEnumerable<TagGridDto> _selectedTags { get; set; } = [];
    
    private List<TagGridDto> _tags { get; set; } = [];
    private TagGridDto? _selectedTag { get; set; }
    
    [Inject] protected NavigationManager NavigationManager { get; set; }
    
    
    // The signature needs to be 'object' rather than void
    private void CreateTag(object obj) => _tagCreator.Show();
    
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
            if (PostId.HasValue)
            {
                _tags = await service.GetTagsByPostAsync(PostId.Value);
            }
            else
            {
                _tags = await service.GetTagsAsync();
            }
            StateHasChanged();
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }

    private async Task DeleteTag(TagGridDto tag)
    {
        try
        {
            await service.DeleteTagAsync(tag.Id);
            await Refresh(); 
        }
        catch (ApiException<ProblemDetails> ex)
        {
            await HandleError(ex);
        }
    }
    
    // Handle row clicks with selection toggle capability
    private void RowClicked(TagGridDto tag)
    {
        var currentList = _selectedTags.ToList();
        if (currentList.Contains(tag))
        {
            currentList.Remove(tag);
            _selectedTag = null;
        }
        else
        {
            _selectedTag = tag;
            currentList.Add(tag);
        }
        
        _selectedTags = currentList;
        StateHasChanged(); 
    }

    private void RowDoubleClicked(TagGridDto tag)
    {
        _selectedTag = tag;
        _selectedTags = new List<TagGridDto> { tag };
        
        EditTag(tag);
    }

    private void SelectedTagsChanged(IEnumerable<TagGridDto> tags)
    {
        _selectedTags = tags;
        _selectedTag = tags.FirstOrDefault();
        StateHasChanged(); 
    }

    private async Task EditTag(TagGridDto tag)
    {
        await _tagEditor.ShowAsync(_selectedTag!.Id);
    }
}