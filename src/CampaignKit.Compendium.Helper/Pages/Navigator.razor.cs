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
        /// Gets or sets the EventCallback for source selection.
        /// </summary>
        [Parameter]
        public EventCallback<SourceDataSet> SelectedSourceChanged { get; set; }

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
        /// Gets or sets the filtered label groups.
        /// </summary>
        /// <value>
        /// The filtered label groups.
        /// </value>
        private List<LabelGroup> FilteredLabelGroups { get; set; } = new List<LabelGroup>();

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
        }

        /// <summary>
        /// This method is called when the component's parameters are set. It calls the FilterLabelGroups method.
        /// </summary>
        protected override void OnParametersSet()
        {
            this.FilterLabelGroups();
        }

        /// <summary>
        /// This method is called when the parameters of the component are set. It clears the
        /// existing filtered label groups and then cycles through each label group. For each label
        /// group, it creates a new filtered label group and adds it to the filtered label groups
        /// list. If the search criteria is empty, it adds all the source data sets of the label
        /// group to the filtered label group. If the search criteria is not empty, it adds only the
        /// source data sets that have a SourceDataSetName matching the search criteria to the
        /// filtered label group.
        /// </summary>
        private void FilterLabelGroups()
        {
            // Clear the existing filtered label groups.
            this.FilteredLabelGroups.Clear();

            // Cycle through each label group and add the label group and its source data sets to the filtered label groups.
            foreach (var labelGroup in this.LabelGroups)
            {
                // Create a new label group.
                var filteredLabelGroup = new LabelGroup
                {
                    LabelName = labelGroup.LabelName,
                    SourceDataSets = new List<SourceDataSet>(),
                };
                this.FilteredLabelGroups.Add(filteredLabelGroup);

                // If search criteria is empty, add the label group and its source data sets to the filtered label groups.
                if (string.IsNullOrEmpty(this.SearchTerm))
                {
                    filteredLabelGroup.SourceDataSets.AddRange(labelGroup.SourceDataSets);
                    continue;
                }
                else
                {
                    // Add source data sets to the filteredLabelGroup that have a SourceDataSetName matching the search criteria.
                    filteredLabelGroup.SourceDataSets.AddRange(labelGroup.SourceDataSets.Where(sds => sds.SourceDataSetName.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase)));
                }
            }
        }

        /// <summary>
        /// Handles the search input change event and filters the tree accordingly.
        /// </summary>
        private void OnSearchChanged(object value)
        {
            this.FilterLabelGroups();
        }

        /// <summary>
        /// Handles the event when a source data set is selected from a menu item.
        /// </summary>
        /// <param name="args">The event arguments containing the selected menu item.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSelectedSourceChanged(MenuItemEventArgs args)
        {
            var source = this.Sources.FirstOrDefault(sds => sds.SourceDataSetName.Equals(args.Text, StringComparison.OrdinalIgnoreCase));
            await this.SelectedSourceChanged.InvokeAsync(source);
        }
    }
}
