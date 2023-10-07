namespace CampaignKit.Compendium.Helper.Shared{    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Represents a dialog box that displays a message to the user.
    /// </summary>
    public partial class MessageDialog    {
        /// <summary>
        /// Gets or sets the prompt for the parameter.
        /// </summary>
        /// <value>The prompt.</value>
        [Parameter]        public string Prompt { get; set; }    }}