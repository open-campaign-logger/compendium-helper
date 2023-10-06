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

namespace CampaignKit.Compendium.Helper.Shared{
    using CampaignKit.Compendium.Helper.Configuration;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Represents a dialog for adding sources.
    /// </summary>
    public partial class AddSourcesDialog    {

        /// <summary>
        /// Gets or sets the Compendium parameter.
        /// </summary>
        /// <value>The Compendium parameter.</value>
        [Parameter]        public ICompendium Compendium { get; set; }

        /// <summary>
        /// Gets or sets the DialogService used for opening dialogs.
        /// </summary>
        [Inject]        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the labels for a specific object.
        /// </summary>
        /// <value>The labels.</value>
        private string Labels { get; set; }

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
        /// Method to handle the "OnAdd" event. 
        /// Splits the "Labels" and "URLs" properties into arrays.
        /// Creates a list to store instances of the "SourceDataSet" class.
        /// Iterates through each URL and creates a new instance of "SourceDataSet" with trimmed URL and labels.
        /// Adds the "SourceDataSet" instance to the list.
        /// Performs further operations with the "sourceDataSets" list.
        /// </summary>
        public void OnAdd()        {            var labels = this.Labels.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // the urls property will have carriage returns in it, so we need to split on that
            var urls = this.URLs.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);            var urlList = new List<string>();            urlList.AddRange(urls);            urlList.Add("*New");            var sourceNumber = 0;

            foreach (var url in urls)            {
                sourceNumber++;                var source = new SourceDataSet()                {
                    SourceDataSetName = $"Source {sourceNumber}",
                    SourceDataSetUri = url.Trim(), // Trim the URL to remove any leading or trailing whitespace
                    Labels = urlList,                };                this.Compendium.SourceDataSets.Add(source); // Add the SourceDataSet instance to the list
            }
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        public void OnCancel()        {            this.DialogService.Close();        }

        /// <summary>
        /// Shows a tooltip for the specified element reference with the given tooltip text and optional tooltip options.
        /// </summary>
        /// <param name="elementReference">The reference to the element for which the tooltip should be shown.</param>
        /// <param name="tooltip">The text to be displayed in the tooltip.</param>
        /// <param name="options">Optional tooltip options to customize the appearance and behavior of the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)        {            this.TooltipService.Open(elementReference, tooltip, options);        }    }}