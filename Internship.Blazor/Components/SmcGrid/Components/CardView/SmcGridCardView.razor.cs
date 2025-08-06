using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Components.SmcGrid.Components.CardView
{
    public partial class SmcGridCardView<TItem> where TItem : class
    {
        [Parameter] public IEnumerable<TItem> Items { get; set; } = new List<TItem>();
        [Parameter] public IEnumerable<TItem> SelectedItems { get; set; } = new List<TItem>();
        [Parameter] public EventCallback<IEnumerable<TItem>> SelectedItemsChanged { get; set; }
        [Parameter] public EventCallback<TItem> OnItemClick { get; set; }
        [Parameter] public EventCallback<TItem> OnItemDoubleClick { get; set; }
        [Parameter] public EventCallback<TItem> OnEditClick { get; set; }
        [Parameter] public EventCallback<TItem> OnDeleteClick { get; set; }
        [Parameter] public bool EditButtonVisible { get; set; } = true;
        [Parameter] public bool DeleteButtonVisible { get; set; } = true;
        [Parameter] public RenderFragment<TItem> CardTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardHeaderTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardBodyTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardFooterTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardActionsTemplate { get; set; }
        
        private async Task CardClicked(TItem item)
        {
            List<TItem> currentSelection = SelectedItems.ToList();
            
            if (currentSelection.Contains(item))
            {
                currentSelection.Remove(item);
            }
            else
            {
                currentSelection.Add(item);
            }
            
            await SelectedItemsChanged.InvokeAsync(currentSelection);
            
            if (OnItemClick.HasDelegate)
            {
                await OnItemClick.InvokeAsync(item);
            }
        }

        private async Task CardDoubleClicked(TItem item)
        {
            if (OnItemDoubleClick.HasDelegate)
            {
                await OnItemDoubleClick.InvokeAsync(item);
            }
        }
        
        private async Task OnEditItemClick(TItem item)
        {
            if (OnEditClick.HasDelegate)
            {
                await OnEditClick.InvokeAsync(item);
            }
        }
        
        private async Task OnDeleteItemClick(TItem item)
        {
            if (OnDeleteClick.HasDelegate)
            {
                await OnDeleteClick.InvokeAsync(item);
            }
        }
    }
}
