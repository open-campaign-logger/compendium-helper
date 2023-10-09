// <copyright file="RemoveLabelsDialog.razor.cs" company="Jochen Linnemann - IT-Service">
// Copyright (c) 2017-2023 Jochen Linnemann, Cory Gill.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace CampaignKit.Compendium.Helper.Dialogs{
    using CampaignKit.Compendium.Helper.Data;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Represents a dialog for removing labels from a user interface.
    /// </summary>
    public partial class RemoveLabelsDialog    {
        /// <summary>
        /// Gets or sets the list of label groups.
        /// </summary>
        /// <value>The list of label groups.</value>
        [Parameter]        public List<LabelGroup> LabelGroups { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when label groups are removed.
        /// </summary>
        /// <value>The event callback for when label groups are removed.</value>
        [Parameter]        public EventCallback<List<LabelGroup>> LabelGroupsRemoved { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets a value indicating whether the SelectedLabelGroups property is not null and contains any elements.
        /// </summary>
        /// <returns>
        /// True if the SelectedLabelGroups property is not null and contains any elements; otherwise, false.
        /// </returns>
        private bool IsValid        {            get            {                return this.SelectedLabelGroups != null && this.SelectedLabelGroups.Any();            }        }

        /// <summary>
        /// Gets or sets the ILogger dependency.
        /// </summary>
        [Inject]        private ILogger<RemoveLabelsDialog> Logger { get; set; }

        /// <summary>
        /// Gets or sets the collection of selected labels.
        /// </summary>
        /// <returns>An IEnumerable of strings representing the selected labels.</returns>
        private IEnumerable<string> SelectedLabelGroups { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService dependency.
        /// </summary>
        [Inject]        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Event handler for when label groups are removed.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnLabelGroupsRemoved()
        {
            this.Logger.LogInformation("User selected OnLabelGroupsRemoved...");

            // Find LabelGroups that correspond to SelectedLabelGroups
            if (this.SelectedLabelGroups != null && this.SelectedLabelGroups.Any())
            {
                // Create a collection of label groups to be removed.
                var labelGroups = this.LabelGroups
                    .Where(labelGroup => this.SelectedLabelGroups.Contains(labelGroup.LabelName))
                    .ToList();

                // Invoke the callback.
                await this.LabelGroupsRemoved.InvokeAsync(labelGroups);
            }

            this.DialogService.Close();
        }

        /// <summary>
        /// Shows a tooltip for the specified element reference with the given tooltip text and optional tooltip options.
        /// </summary>
        /// <param name="elementReference">The reference to the element for which the tooltip should be shown.</param>
        /// <param name="tooltip">The text to be displayed in the tooltip.</param>
        /// <param name="options">Optional tooltip options to customize the appearance and behavior of the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)        {            this.TooltipService.Open(elementReference, tooltip, options);        }    }}