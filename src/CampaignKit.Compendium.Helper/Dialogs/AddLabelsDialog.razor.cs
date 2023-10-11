// <copyright file="AddLabelsDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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

    using Microsoft.AspNetCore.Components;    using Radzen;

    /// <summary>
    /// Represents a dialog for adding a label.
    /// </summary>
    public partial class AddLabelsDialog    {
        /// <summary>
        /// Gets or sets the LabelGroups parameter.
        /// </summary>
        [Parameter]
        public List<LabelGroup> LabelGroups { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when a label is added.
        /// The event callback takes a list of strings as a parameter.
        /// </summary>
        [Parameter]        public EventCallback<List<LabelGroup>> LabelGroupsAdded { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets a value indicating whether the Labels property is not null or empty.
        /// </summary>
        /// <returns>
        /// True if the Labels property is not null or empty, otherwise false.
        /// </returns>
        private bool IsValid        {            get            {
                // return true if SelectedLabelGroups is not null or empty.
                return this.Labels != null && this.Labels.Split(',', StringSplitOptions.RemoveEmptyEntries).Any();            }        }

        /// <summary>
        /// Gets or sets the labels for a specific object.
        /// </summary>
        /// <value>The labels.</value>
        private string Labels { get; set; }

        /// <summary>
        /// Gets or sets injects the ILogger dependency.
        /// </summary>
        [Inject]        private ILogger<AddLabelsDialog> Logger { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService dependency.
        /// </summary>
        [Inject]        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Event handler for adding labels. Logs the event, invokes the LabelGroupsAdded event with the list of labels, and closes the dialog.
        /// </summary>
        private async Task OnLabelGroupsAdded()        {            this.Logger.LogInformation("User selected OnLabelGroupsAdded...");

            if (!string.IsNullOrEmpty(this.Labels))
            {
                // Split label selections into a list of strings.
                var labels = this.Labels.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

                // Remove any labels that are already associated with a collection of source data sets in Sources.
                labels.RemoveAll(label => this.LabelGroups.Any(group => group.LabelName.Equals(label)));

                // Remove any labels that match "*No Label"
                labels.RemoveAll(label => label.Equals("*No Label"));                

                // Create the required label groups
                var labelGroups = labels.Select(label => new LabelGroup
                {
                    LabelName = label,
                    SourceDataSets = new List<Configuration.SourceDataSet>(),
                }).ToList();

                // Invoke the callback
                await this.LabelGroupsAdded.InvokeAsync(labelGroups);
            }
            // Close the dialog.            this.DialogService.Close();        }

        /// <summary>
        /// Shows a tooltip for the specified element reference with the given tooltip text and optional tooltip options.
        /// </summary>
        /// <param name="elementReference">The reference to the element for which the tooltip should be shown.</param>
        /// <param name="tooltip">The text to be displayed in the tooltip.</param>
        /// <param name="options">Optional tooltip options to customize the appearance and behavior of the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)        {            this.TooltipService.Open(elementReference, tooltip, options);        }    }}