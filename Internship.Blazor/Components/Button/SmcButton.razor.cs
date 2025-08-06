using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Internship.Blazor.Components.Button
{
    public enum ButtonTheme
    {
        Default,
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark,
        Link
    }

    public enum ButtonSize
    {
        Small,
        Medium,
        Large
    }

    public enum ButtonShape
    {
        Rectangular,
        Round
    }

    public enum ButtonType
    {
        Button,
        Submit,
        Reset
    }

    public partial class SmcButton
    {
        [Parameter] public string Id { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public string IconClass { get; set; }
        [Parameter] public string Icon { get; set; } // Telerik icon (if needed)
        [Parameter] public ButtonTheme Theme { get; set; } = ButtonTheme.Default;
        [Parameter] public ButtonSize Size { get; set; } = ButtonSize.Medium;
        [Parameter] public ButtonShape Shape { get; set; } = ButtonShape.Rectangular;
        [Parameter] public ButtonType ButtonType { get; set; } = ButtonType.Button;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool Outlined { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool PreventDefault { get; set; }
        [Parameter] public bool StopPropagation { get; set; }
        [Parameter] public bool IconOnly { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        [Parameter] public string CustomClass { get; set; }
        [Parameter] public string CustomStyle { get; set; }

        protected string CssClass => BuildCssClass();

        private string BuildCssClass()
        {
            var cssClass = "smc-button";

            // Theme styling
            cssClass += Theme switch
            {
                ButtonTheme.Primary => " smc-button-primary",
                ButtonTheme.Secondary => " smc-button-secondary",
                ButtonTheme.Success => " smc-button-success",
                ButtonTheme.Danger => " smc-button-danger",
                ButtonTheme.Warning => " smc-button-warning",
                ButtonTheme.Info => " smc-button-info",
                ButtonTheme.Light => " smc-button-light",
                ButtonTheme.Dark => " smc-button-dark",
                ButtonTheme.Link => " smc-button-link",
                _ => ""
            };

            // Size
            cssClass += Size switch
            {
                ButtonSize.Small => " smc-button-sm",
                ButtonSize.Medium => "", // Default
                ButtonSize.Large => " smc-button-lg",
                _ => ""
            };

            // Shape
            cssClass += Shape switch
            {
                ButtonShape.Rectangular => "", // Default
                ButtonShape.Round => " smc-button-round",
                _ => ""
            };

            // Outlined style
            if (Outlined)
                cssClass += " smc-button-outlined";

            // Icon-only button
            if (IconOnly || (string.IsNullOrEmpty(Text) && ChildContent == null && (!string.IsNullOrEmpty(IconClass) || !string.IsNullOrEmpty(Icon))))
                cssClass += " smc-button-icon-only";

            // Add custom class
            if (!string.IsNullOrEmpty(CustomClass))
                cssClass += " " + CustomClass;

            return cssClass;
        }

        protected string GetHtmlButtonType()
        {
            return ButtonType switch
            {
                ButtonType.Submit => "submit",
                ButtonType.Reset => "reset",
                _ => "button"
            };
        }

        public async Task HandleClick(MouseEventArgs args)
        {
            if (!Disabled && OnClick.HasDelegate)
                await OnClick.InvokeAsync(args);
        }
    }
}
