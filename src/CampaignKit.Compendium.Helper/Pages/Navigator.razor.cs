// <copyright file="Navigator.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Pages
{
    using System;

    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Data;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Represents a partial class for the Navigator component.
    /// </summary>
    public partial class Navigator
    {
        /// <summary>
        /// Gets or sets the LabelGroups parameter.
        /// </summary>
        [Parameter]
        public List<LabelGroup> LabelGroups { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when a label group is changed.
        /// </summary>
        /// <value>The event callback for when a label group is changed.</value>
        [Parameter]
        public EventCallback<LabelGroup> SelectedLabelGroupChanged { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for source selection.
        /// </summary>
        [Parameter]
        public EventCallback<(SourceDataSet, LabelGroup)> SelectedSourceChanged { get; set; }

        /// <summary>
        /// Gets or sets the Sources parameter.
        /// </summary>
        [Parameter]
        public List<SourceDataSet> Sources { get; set; }

        /// <summary>
        /// Gets a list of distinct source data set names from the compendium.
        /// </summary>
        /// <returns>A list of distinct source data set names.</returns>
        private IEnumerable<string> AutoCompleteData
        {
            get
            {
                // Return a list of distinct source data set names from the compendium ordered alphabetically.
                return this.Sources.Select(sds => sds.SourceDataSetName).Distinct().OrderBy(sds => sds);
            }
        }

        /// <summary>
        /// Gets or sets the filtered list of LabelGroups.
        /// </summary>
        private List<LabelGroup> FilteredLabelGroups { get; set; } = new ();

        /// <summary>
        /// Gets or sets the filtered source data sets.
        /// </summary>
        /// <value>
        /// The filtered source data sets.
        /// </value>
        private List<SourceDataSet> FilteredSourceDataSets { get; set; } = new ();

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        private string SearchTerm { get; set; } = string.Empty;

        /// <summary>
        /// Initializes the component asynchronously.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.SearchTerm = string.Empty;
            this.FilterLabelGroups();
        }

        /// <summary>
        /// Overrides the OnParametersSetAsync method and calls the base implementation before filtering the tree.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous operation.
        /// </returns>
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            this.FilterLabelGroups();
        }

        /// <summary>
        /// Filters the tree based on the search term.
        /// </summary>
        private void FilterLabelGroups()
        {
            if (string.IsNullOrWhiteSpace(this.SearchTerm))
            {
                this.FilteredSourceDataSets.Clear();
                this.FilteredSourceDataSets.AddRange(this.Sources);
            }
            else
            {
                // Filter source data sets based on search criteria
                this.FilteredSourceDataSets =
                    this.Sources
                    .Where(sds => sds.SourceDataSetName
                        .Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Create a variable to hold label groups with no SourceDataSets
            var emptyLabelGroups = this.LabelGroups?
                .Where(labelGroup => !labelGroup.SourceDataSets.Any()).ToList()
                    ?? new List<LabelGroup>();

            // Create a list of label groups based on the filtered source data sets
            // Create LabelGroups for Labels in use by Sources
            this.FilteredLabelGroups = this.FilteredSourceDataSets
                .SelectMany(
                    ds => ds.Labels.Any()
                        ? ds.Labels.Select(label => new { Label = label, DataSet = ds })
                        : new[] { new { Label = "*No Label", DataSet = ds } })
                .GroupBy(pair => pair.Label)
                .Select(group => new LabelGroup
                {
                    LabelName = group.Key,
                    SourceDataSets = group.Select(pair => pair.DataSet).OrderBy(sds => sds.SourceDataSetName)
                        .ToList(),
                })
                .Concat(emptyLabelGroups)
                .OrderBy(group => group.LabelName)
                .ToList();
        }

        /// <summary>
        /// Handles the search input change event and filters the tree accordingly.
        /// </summary>
        private void OnSearchChanged(object value)
        {
            this.FilterLabelGroups();
        }

        /// <summary>
        /// Event handler for when the expanded state of a label changes.
        /// </summary>
        /// <param name="labelGroup">The label group that contains the label.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSelectedLabelGroupChanged(LabelGroup labelGroup)
        {
            // Retrieve the unfiltered label group from the LabelGroups list
            var unfilteredLabelGroup = this.LabelGroups.FirstOrDefault(lg => lg.LabelName.Equals(labelGroup.LabelName));
            await this.SelectedLabelGroupChanged.InvokeAsync(unfilteredLabelGroup);
        }

        /// <summary>
        /// Handles the event when a source data set is selected from a menu item.
        /// </summary>
        /// <param name="args">The event arguments containing the selected menu item.</param>
        /// <param name="labelGroup">The label group associated with the selected source data set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSelectedSourceChanged(MenuItemEventArgs args, LabelGroup labelGroup)
        {
            // Retrieve the SourceDataSet from the compendium by its name
            var sourceDataSet = this.FilteredSourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(args.Text, StringComparison.OrdinalIgnoreCase));

            // Retrieve the unfiltered label group from the LabelGroups list
            var unfilteredLabelGroup = this.LabelGroups.FirstOrDefault(lg => lg.LabelName.Equals(labelGroup.LabelName));

            // Invoke the SourceSelected event
            await this.SelectedSourceChanged.InvokeAsync((sourceDataSet, unfilteredLabelGroup));
        }
    }
}
