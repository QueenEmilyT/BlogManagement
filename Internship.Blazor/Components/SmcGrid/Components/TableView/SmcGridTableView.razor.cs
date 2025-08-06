using Microsoft.AspNetCore.Components;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace Internship.Blazor.Components.SmcGrid.Components.TableView
{
    public partial class SmcGridTableView<TItem> where TItem : class
    {
        [Parameter] public IEnumerable<TItem> Items { get; set; } = new List<TItem>();
        [Parameter] public IEnumerable<TItem> SelectedItems { get; set; } = new List<TItem>();
        [Parameter] public EventCallback<IEnumerable<TItem>> SelectedItemsChanged { get; set; }
        [Parameter] public bool ShowCheckboxColumn { get; set; } = true;
        [Parameter] public bool EnableLoaderContainer { get; set; } = true;
        [Parameter] public bool ConfirmDelete { get; set; } = true;
        [Parameter] public bool Pageable { get; set; } = true;
        [Parameter] public bool Groupable { get; set; } = true;
        [Parameter] public bool Sortable { get; set; } = true;
        [Parameter] public GridFilterMode FilterMode { get; set; } = GridFilterMode.FilterMenu;
        [Parameter] public bool Resizable { get; set; } = true;
        [Parameter] public bool Reorderable { get; set; } = true;
        [Parameter] public GridEditMode EditMode { get; set; } = GridEditMode.Popup;
        [Parameter] public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.Multiple;
        [Parameter] public int PageSize { get; set; } = 10;
        [Parameter] public bool Navigable { get; set; } = true;
        [Parameter] public RenderFragment ColumnsTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> ActionsColumnTemplate { get; set; }
        [Parameter] public EventCallback<GridRowClickEventArgs> OnRowClick { get; set; }
        [Parameter] public EventCallback<GridRowClickEventArgs> OnRowDoubleClick { get; set; }

        [Parameter] public TelerikGrid<TItem> GridRef { get; set; }
    }
}
