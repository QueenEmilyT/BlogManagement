using Internship.Blazor.Components.SmcGrid.Components;
using Internship.Blazor.Components.SmcGrid.Models;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace Internship.Blazor.Components.SmcGrid
{
    public partial class SmcGrid<TItem> where TItem : class
    {
        #region Parameters
        [Parameter] public string Title { get; set; } = "Items";
        [Parameter] public string SearchPlaceholder { get; set; } = "Search...";
        [Parameter] public bool CreateButtonVisible { get; set; } = true;
        [Parameter] public string CreateButtonText { get; set; } = "Create";
        [Parameter] public EventCallback OnCreateClick { get; set; }
        [Parameter] public EventCallback<TItem> OnEditClick { get; set; }
        [Parameter] public EventCallback<TItem> OnDeleteClick { get; set; }
        [Parameter] public EventCallback<TItem> OnRowClick { get; set; }
        [Parameter] public EventCallback<TItem> OnRowDoubleClick { get; set; }
        [Parameter] public EventCallback OnRefresh { get; set; }
        
        // Event handlers for standard dropdown actions
        [Parameter] public EventCallback OnExportClick { get; set; }
        
        [Parameter] public bool EnableGridLoaderContainer { get; set; } = true;
        [Parameter] public bool LoaderContainerVisible { get; set; } = true;
        [Parameter] public IEnumerable<TItem> Items { get; set; } = new List<TItem>();
        [Parameter] public IEnumerable<TItem> SelectedItems { get; set; } = new List<TItem>();
        [Parameter] public EventCallback<IEnumerable<TItem>> SelectedItemsChanged { get; set; }
        [Parameter] public EventCallback<IEnumerable<TItem>> OnSelectedItemsChanged { get; set; }
        
        [Parameter] public bool EditButtonVisible { get; set; } = true;
        [Parameter] public bool DeleteButtonVisible { get; set; } = true;
        [Parameter] public bool ShowCheckboxColumn { get; set; } = true;
        
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
        
        [Parameter] public Func<TItem, string> SearchPredicate { get; set; }
        [Parameter] public RenderFragment ColumnsTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> ActionsColumnTemplate { get; set; }
        [Parameter] public RenderFragment HeaderRightContentTemplate { get; set; }
        [Parameter] public RenderFragment ToolbarMenusTemplate { get; set; }
        [Parameter] public RenderFragment ToolbarLeftContentTemplate { get; set; }
        [Parameter] public RenderFragment ToolbarRightContentTemplate { get; set; }
        [Parameter] public RenderFragment ToolbarRightDropdownTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardHeaderTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardBodyTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardFooterTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> CardActionsTemplate { get; set; }
        [Parameter] public RenderFragment LoadingTemplate { get; set; }
        #endregion
        
        // Current view state
        private SmcGridViewType CurrentView { get; set; } = SmcGridViewType.Table;
        
        // Component references
        private SmcGridToolbar ToolbarRef { get; set; }
        
        // Search functionality
        private string SearchTerm { get; set; } = string.Empty;
        
        // Grid reference
        public TelerikGrid<TItem> GridRef { get; set; }

        // Track whether selection has changed
        private bool _selectionChanged = false;
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            
            // This will ensure the toolbar is updated when SelectedItems changes externally
            if (_selectionChanged)
            {
                _selectionChanged = false;
                StateHasChanged();
            }
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            
            // You could implement click outside handling here to close dropdowns
            // Or add JS interop for global click handling
        }
        
        public async Task Refresh()
        {
            LoaderContainerVisible = true;
            if (OnRefresh.HasDelegate)
            {
                await OnRefresh.InvokeAsync();
            }
            LoaderContainerVisible = false;
            StateHasChanged();
        }
        
        // Close all dropdowns in the grid
        public void CloseAllDropdowns()
        {
            ToolbarRef?.CloseAllDropdowns();
            StateHasChanged();
        }
        
        private IEnumerable<TItem> FilteredItems
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchTerm) || SearchPredicate == null)
                    return Items ?? new List<TItem>();

                return Items?.Where(item => 
                    SearchPredicate(item)?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true
                ) ?? new List<TItem>();
            }
        }

        #region Event Handlers
        private void HandleViewChange(SmcGridViewType viewType)
        {
            CurrentView = viewType;
            StateHasChanged();
        }

        private async void RowClicked(GridRowClickEventArgs args)
        {
            if (args?.Item is TItem item && OnRowClick.HasDelegate)
            {
                await OnRowClick.InvokeAsync(item);
                _selectionChanged = true;
                StateHasChanged();
            }
        }

        private async void RowDoubleClicked(GridRowClickEventArgs args)
        {
            if (args?.Item is TItem item && OnRowDoubleClick.HasDelegate)
            {
                await OnRowDoubleClick.InvokeAsync(item);
                _selectionChanged = true;
                StateHasChanged();
            }
        }

        private async Task HandleCardClick(TItem item)
        {
            if (OnRowClick.HasDelegate)
            {
                await OnRowClick.InvokeAsync(item);
                _selectionChanged = true;
                StateHasChanged();
            }
        }

        private async Task HandleCardDoubleClick(TItem item)
        {
            if (OnRowDoubleClick.HasDelegate)
            {
                await OnRowDoubleClick.InvokeAsync(item);
                _selectionChanged = true;
                StateHasChanged();
            }
        }
        
        // Handle SelectedItemsChanged event internally
        private async Task HandleSelectedItemsChanged(IEnumerable<TItem> items)
        {
            if (SelectedItemsChanged.HasDelegate)
            {
                await SelectedItemsChanged.InvokeAsync(items);
                _selectionChanged = true;
            }
            
            if (OnSelectedItemsChanged.HasDelegate)
            {
                await OnSelectedItemsChanged.InvokeAsync(items);
                _selectionChanged = true;
            }
            
            if (_selectionChanged)
            {
                StateHasChanged();
            }
        }
        
        private async Task HandleEditClick()
        {
            if (OnEditClick.HasDelegate && SelectedItems.Any())
            {
                await OnEditClick.InvokeAsync(SelectedItems.First());
            }
        }
        
        private async Task HandleDeleteClick()
        {
            if (OnDeleteClick.HasDelegate && SelectedItems.Any())
            {
                await OnDeleteClick.InvokeAsync(SelectedItems.First());
            }
        }
        
        private async Task HandleItemEdit(TItem item)
        {
            if (OnEditClick.HasDelegate)
            {
                await OnEditClick.InvokeAsync(item);
            }
        }
        
        private async Task HandleItemDelete(TItem item)
        {
            if (OnDeleteClick.HasDelegate)
            {
                await OnDeleteClick.InvokeAsync(item);
            }
        }
        
        private async Task HandleExportClick()
        {
            if (OnExportClick.HasDelegate)
            {
                await OnExportClick.InvokeAsync();
            }
            ToolbarRef?.CloseAllDropdowns();
        }
        #endregion
    }
}
