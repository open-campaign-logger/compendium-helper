using Microsoft.AspNetCore.Components;using Microsoft.AspNetCore.Components.Web;

using Radzen;namespace CampaignKit.Compendium.Helper.Dialogs{
    /// <summary>
    /// Represents a dialog for removing labels from a user interface.
    /// </summary>
    public partial class RemoveLabelsDialog    {
        /// <summary>
        /// Gets or sets the list of all labels.
        /// </summary>
        /// <value>The list of all labels.</value>
        [Parameter]        public List<string> AllLabels { get; set; }

        /// <summary>
        /// Gets or sets the ILogger dependency.
        /// </summary>
        [Inject]        private ILogger<RemoveLabelsDialog> Logger { get; set; }

        /// <summary>
        /// Gets or sets the collection of selected labels.
        /// </summary>
        /// <returns>An IEnumerable of strings representing the selected labels.</returns>
        private IEnumerable<string> SelectedLabels { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService dependency.
        /// </summary>
        [Inject]        private TooltipService TooltipService { get; set; }

        private bool IsValid
        {
            get
            {
                return this.SelectedLabels != null && this.SelectedLabels.Any();
            }
        }

        private void OnRemove(MouseEventArgs args)
        {
            this.Logger.LogInformation("User selected OnRemove...");
        }

        /// <summary>
        /// Shows a tooltip for the specified element reference with the given tooltip text and optional tooltip options.
        /// </summary>
        /// <param name="elementReference">The reference to the element for which the tooltip should be shown.</param>
        /// <param name="tooltip">The text to be displayed in the tooltip.</param>
        /// <param name="options">Optional tooltip options to customize the appearance and behavior of the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)        {            this.TooltipService.Open(elementReference, tooltip, options);        }    }}