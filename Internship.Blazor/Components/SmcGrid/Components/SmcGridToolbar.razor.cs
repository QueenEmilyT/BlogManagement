using Internship.Blazor.Components.SmcGrid.Models;
using Microsoft.AspNetCore.Components;

namespace Internship.Blazor.Components.SmcGrid.Components
{
    public partial class SmcGridToolbar
    {
        [Parameter] public SmcGridViewType CurrentView { get; set; } = SmcGridViewType.Table;
        [Parameter] public EventCallback<SmcGridViewType> CurrentViewChanged { get; set; }
        [Parameter] public bool HasSelectedItems { get; set; } = false;
        [Parameter] public EventCallback OnRefresh { get; set; }
        [Parameter] public EventCallback OnEditClick { get; set; }
        [Parameter] public EventCallback OnDeleteClick { get; set; }
        [Parameter] public EventCallback OnServicesClick { get; set; }
        [Parameter] public EventCallback OnProjectsClick { get; set; }
        [Parameter] public EventCallback OnExportClick { get; set; }
        [Parameter] public bool EditButtonVisible { get; set; } = true;
        [Parameter] public bool DeleteButtonVisible { get; set; } = true;
        [Parameter] public RenderFragment ToolbarMenusTemplate { get; set; }
        [Parameter] public RenderFragment ToolbarLeftContentTemplate { get; set; }
        [Parameter] public RenderFragment ToolbarRightContentTemplate { get; set; }
        [Parameter] public RenderFragment ToolbarRightDropdownTemplate { get; set; }
        
        // Dropdown state
        public bool ShowActionsDropdown { get; set; } = false;

        private async Task SwitchView(SmcGridViewType viewType)
        {
            if (CurrentView != viewType)
            {
                CurrentView = viewType;
                await CurrentViewChanged.InvokeAsync(viewType);
            }
        }
        
        public void ToggleActionsDropdown()
        {
            ShowActionsDropdown = !ShowActionsDropdown;
            StateHasChanged();
        }
        
        public void CloseAllDropdowns()
        {
            ShowActionsDropdown = false;
            StateHasChanged();
        }
    }
}