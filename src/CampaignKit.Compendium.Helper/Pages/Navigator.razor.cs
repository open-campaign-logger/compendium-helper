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
    using System.Runtime.CompilerServices;

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
        /// Gets or sets the AllLabelGroups parameter.
        /// </summary>
        [Parameter]
        public List<LabelGroup> AllLabelGroups { get; set; }

        /// <summary>
        /// Gets or sets the AllSourceDataSets parameter.
        /// </summary>
        [Parameter]
        public List<SourceDataSet> AllSourceDataSets { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for the compendium collapsed event.
        /// </summary>
        [Parameter]
        public EventCallback<ICompendium> CompendiumCollapsed { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for the compendium expansion event.
        /// </summary>
        [Parameter]
        public EventCallback<ICompendium> CompendiumExpanded { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for the label collapsed event.
        /// </summary>
        [Parameter]
        public EventCallback<LabelGroup> LabelCollapsed { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for the label expansion event.
        /// </summary>
        [Parameter]
        public EventCallback<LabelGroup> LabelExpanded { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for source selection.
        /// </summary>
        [Parameter]
        public EventCallback<(SourceDataSet, LabelGroup)> SourceSelected { get; set; }

        /// <summary>
        /// Gets or sets the list of temporary labels that have no corresponding SourceDataSets.
        /// </summary>
        [Parameter]
        public List<string> TemporaryLabels { get; set; }

        /// <summary>
        /// Gets a list of distinct source data set names from the compendium.
        /// </summary>
        /// <returns>A list of distinct source data set names.</returns>
        private IEnumerable<string> AutoCompleteData
        {
            get
            {
                // Return a list of distinct source data set names from the compendium ordered alphabetically.
                return this.AllSourceDataSets.Select(sds => sds.SourceDataSetName).Distinct().OrderBy(sds => sds);
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
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.SearchTerm = string.Empty;
            this.FilterTree();
        }

        /// <summary>
        /// Overrides the OnParametersSetAsync method to perform additional logic before the component's parameters are set.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous operation.
        /// </returns>
        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            this.FilterTree();
        }

        /// <summary>
        /// Filters the tree based on the search term.
        /// </summary>
        private void FilterTree()
        {
            if (string.IsNullOrWhiteSpace(this.SearchTerm))
            {
                this.FilteredSourceDataSets.Clear();
                this.FilteredSourceDataSets.AddRange(this.AllSourceDataSets);
                this.FilteredLabelGroups.Clear();
                this.FilteredLabelGroups.AddRange(this.AllLabelGroups);
            }
            else
            {
                // Filter source data sets based on search criteria
                this.FilteredSourceDataSets =
                    this.AllSourceDataSets
                    .Where(sds => sds.SourceDataSetName.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

                // Filter label groups based on search criteria.  If a label group contains a source data set that has a SourceDataSetName matching the search criteria, add the label group to the filtered list.
                this.FilteredLabelGroups =
                    this.AllLabelGroups
                    .Where(lg => lg.SourceDataSets.Any(sds => sds.SourceDataSetName.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase))).ToList();
            }
        }

        /// <summary>
        /// Event handler for the CompendiumExpandedChanged event. Invokes the CompendiumExpanded event with the specified compendium.
        /// </summary>
        /// <param name="isExpanded">A boolean indicating whether the compendium is expanded or collapsed.</param>
        /// <param name="compendium">The compendium that was expanded or collapsed.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task OnCompendiumExpandedChanged(bool isExpanded, ICompendium compendium)
        {
            await this.CompendiumExpanded.InvokeAsync(compendium);
        }

        /// <summary>
        /// Event handler for when the expanded state of a label changes.
        /// </summary>
        /// <param name="isExpanded">The new expanded state of the label.</param>
        /// <param name="labelGroup">The label group that contains the label.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnLabelExpandedChanged(bool isExpanded, LabelGroup labelGroup)
        {
            // Retrieve the label group that contains the label
            await this.LabelExpanded.InvokeAsync(labelGroup);
        }

        /// <summary>
        /// Handles the search input change event and filters the tree accordingly.
        /// </summary>
        private void OnSearchChanged(object value)
        {
            this.FilterTree();
        }

        /// <summary>
        /// Handles the event when a source data set is selected from a menu item.
        /// </summary>
        /// <param name="args">The event arguments containing the selected menu item.</param>
        /// <param name="labelGroup">The label group associated with the selected source data set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSourceDataSetSelected(MenuItemEventArgs args, LabelGroup labelGroup)
        {
            // Retrieve the SourceDataSet from the compendium by its name
            var sourceDataSet = this.FilteredSourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(args.Text, StringComparison.OrdinalIgnoreCase));

            // Invoke the SourceSelected event
            await this.SourceSelected.InvokeAsync((sourceDataSet, labelGroup));
        }
    }
}
