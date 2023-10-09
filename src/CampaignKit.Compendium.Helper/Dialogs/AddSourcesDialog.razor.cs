// <copyright file="AddSourcesDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Data;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Represents a dialog for adding sources.
    /// </summary>
    public partial class AddSourcesDialog    {
        /// <summary>
        /// Gets or sets the list of all available SourceDataSet objects.
        /// </summary>
        [Parameter]        public List<SourceDataSet> Sources { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when sources are removed.
        /// </summary>
        /// <value>The event callback for when sources are removed.</value>
        [Parameter]
        public EventCallback<List<SourceDataSet>> SourcesAdded { get; set; }

        /// <summary>
        /// Gets or sets the DialogService used for opening dialogs.
        /// </summary>
        [Inject]        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets a value indicating whether the URLs property and Tag property are not null or empty.
        /// </summary>
        /// <returns>
        /// True if the URLs property and Tag property are not null or empty, otherwise false.
        /// </returns>
        private bool IsValid
        {
            get
            {
                // Return true if the URLs property is not null or empty and a Tag is selected.
                return !string.IsNullOrEmpty(this.URLs) && !string.IsNullOrEmpty(this.Tag);
            }
        }

        /// <summary>
        /// Gets or sets the labels for a specific object.
        /// </summary>
        /// <value>The labels.</value>
        private string Labels { get; set; }

        /// <summary>
        /// Gets or sets the tag to apply to the sources.
        /// </summary>
        private string Tag { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService used for hover-over help popups.
        /// </summary>
        [Inject] // TooltipService
        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets or sets the URLs string.
        /// </summary>
        /// <value>The URLs string.</value>
        private string URLs { get; set; }

        /// <summary>
        /// Method to handle the event when adding sources.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task OnSourcesAdded()        {
            // Assemble the label list from the provided values.
            var labelList = this.Labels?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>();

            // the urls property will have carriage returns in it, so we need to split on that
            var urls = this.URLs.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);            var sources = new List<SourceDataSet>();

            foreach (var url in urls)            {
                // Determine the next available source dataset name.
                var sourceDataSetName = "SelectedSource";                var sourceDataSetNumber = 1;                while (this.Sources.Any(sds => sds.SourceDataSetName.Equals($"{sourceDataSetName} {sourceDataSetNumber}")))
                {
                    sourceDataSetNumber++;
                }

                // Add the new source to the collection.
                sources.Add(new SourceDataSet()                {
                    SourceDataSetName = $"{sourceDataSetName} {sourceDataSetNumber}",
                    SourceDataSetUri = url.Trim(), // Trim the URL to remove any leading or trailing whitespace
                    Labels = labelList,                    TagSymbol = this.Tag,                });
            }

            await this.SourcesAdded.InvokeAsync(sources);

            // Close the dialog.
            this.DialogService.Close();
        }

        /// <summary>
        /// Shows a tooltip for the specified element reference with the given tooltip text and optional tooltip options.
        /// </summary>
        /// <param name="elementReference">The reference to the element for which the tooltip should be shown.</param>
        /// <param name="tooltip">The text to be displayed in the tooltip.</param>
        /// <param name="options">Optional tooltip options to customize the appearance and behavior of the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)        {            this.TooltipService.Open(elementReference, tooltip, options);        }    }}