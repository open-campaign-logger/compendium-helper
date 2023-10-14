﻿// <copyright file="RemoveSourcesDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Dialogs
{
    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Data;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Code behind for the RemoveSourcesDialog component.
    /// </summary>
    public partial class RemoveSourcesDialog
    {
        /// <summary>
        /// Gets or sets the list of all available SourceDataSet objects.
        /// </summary>
        [Parameter]
        public List<SourceDataSet> Sources { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when sources are removed.
        /// </summary>
        /// <value>The event callback for when sources are removed.</value>
        [Parameter]
        public EventCallback<List<SourceDataSet>> SourcesRemoved { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]
        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets a value indicating whether the SelectedLabelGroups property is not null or empty.
        /// </summary>
        /// <returns>
        /// Returns true if SelectedLabelGroups is not null or empty, otherwise false.
        /// </returns>
        private bool IsValid
        {
            get
            {
                // return true if SelectedLabelGroups is not null or empty.
                return this.SelectedDataSets != null && this.SelectedDataSets.Any();
            }
        }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<RemoveSourcesDialog> Logger { get; set; }

        /// <summary>
        /// Gets or sets the values of the selected data sets.
        /// </summary>
        private IEnumerable<string> SelectedDataSets { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService dependency.
        /// </summary>
        [Inject]
        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Sorts the source data sets and gets the list of selected data sets.
        /// </summary>
        /// <returns>
        /// A list of strings containing the selected data sets.
        /// </returns>
        protected override async Task OnParametersSetAsync()
        {
            this.Logger.LogInformation("OnParametersSetAsync");
            await base.OnParametersSetAsync();

            // Sort the source data sets.
            this.Sources.Sort((x, y) => string.Compare(x.SourceDataSetName, y.SourceDataSetName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Removes the selected data sets from the list of all sources and raises the SourceRemoved event.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnSourcesRemoved()
        {
            // Get a list of sources to be removed.
            var sources = this.Sources.Where(source => this.SelectedDataSets.Contains(source.SourceDataSetName)).ToList();

            // Raise the SourcesRemoved event.
            await this.SourcesRemoved.InvokeAsync(sources);

            // Close the dialog box.
            this.DialogService.Close();
        }

        /// <summary>
        /// Shows a tooltip for the specified element reference with the given tooltip text and optional tooltip options.
        /// </summary>
        /// <param name="elementReference">The reference to the element for which the tooltip should be shown.</param>
        /// <param name="tooltip">The text to be displayed in the tooltip.</param>
        /// <param name="options">Optional tooltip options to customize the appearance and behavior of the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)
        {
            this.TooltipService.Open(elementReference, tooltip, options);
        }
    }
}
