using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Components.SmcGrid.Components
{
    public partial class SmcGridHeader
    {
        [Parameter] public string Title { get; set; } = "Items";
        [Parameter] public string SearchPlaceholder { get; set; } = "Search...";
        [Parameter] public string SearchTerm { get; set; }
        [Parameter] public EventCallback<string> SearchTermChanged { get; set; }
        [Parameter] public bool CreateButtonVisible { get; set; } = true;
        [Parameter] public string CreateButtonText { get; set; } = "Create";
        [Parameter] public EventCallback OnCreateClick { get; set; }
        [Parameter] public RenderFragment HeaderRightContentTemplate { get; set; }

        private async Task OnSearchInput(ChangeEventArgs e)
        {
            SearchTerm = e.Value?.ToString() ?? string.Empty;
            await SearchTermChanged.InvokeAsync(SearchTerm);
        }
    }
}
