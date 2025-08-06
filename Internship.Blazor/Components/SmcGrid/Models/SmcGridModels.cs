namespace Internship.Blazor.Components.SmcGrid.Models
{
    /// <summary>
    /// Defines the available view modes for the SmcGrid
    /// </summary>
    public enum SmcGridViewType 
    { 
        /// <summary>
        /// Traditional table/grid view
        /// </summary>
        Table, 
        
        /// <summary>
        /// Card-based view for more visual representation
        /// </summary>
        Board 
    }
    
    /// <summary>
    /// Configuration options for the SmcGrid component
    /// </summary>
    public class SmcGridOptions
    {
        /// <summary>
        /// Title displayed in the grid header
        /// </summary>
        public string Title { get; set; } = "Items";
        
        /// <summary>
        /// Placeholder text for the search input
        /// </summary>
        public string SearchPlaceholder { get; set; } = "Search...";
        
        /// <summary>
        /// Text displayed on the create button
        /// </summary>
        public string CreateButtonText { get; set; } = "Create";
        
        /// <summary>
        /// Whether to show the create button
        /// </summary>
        public bool CreateButtonVisible { get; set; } = true;
        
        /// <summary>
        /// Whether to show the edit button
        /// </summary>
        public bool EditButtonVisible { get; set; } = true;
        
        /// <summary>
        /// Whether to show the delete button
        /// </summary>
        public bool DeleteButtonVisible { get; set; } = true;
        
        /// <summary>
        /// Whether to show checkbox column in table view
        /// </summary>
        public bool ShowCheckboxColumn { get; set; } = true;
    }
}

