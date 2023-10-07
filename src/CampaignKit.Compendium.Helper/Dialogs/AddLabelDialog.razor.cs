using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace CampaignKit.Compendium.Helper.Dialogs
{
    public partial class AddLabelDialog
    {
        /// <summary>
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        private TooltipService TooltipService { get; set; }


        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<AddLabelDialog> Logger { get; set; }

        private string Labels { get; set; }

        private bool IsValid
        {
            get
            {
                // return true if SelectedDataSets is not null or empty.
                return this.Labels != null && this.Labels.Split(',', StringSplitOptions.RemoveEmptyEntries).Any();
            }
        }


        private void OnAdd(MouseEventArgs args)
        {
            this.Logger.LogInformation("OnAdd");
        }

        /// <summary>
        /// Opens a tooltip with the specified content for the given element.
        /// </summary>
        /// <param name="elementReference">The element to open the tooltip for.</param>
        /// <param name="tooltip">The tooltip content.</param>
        /// <param name="options">Optional options for the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)
        {
            this.TooltipService.Open(elementReference, tooltip, options);
        }
    }
}
