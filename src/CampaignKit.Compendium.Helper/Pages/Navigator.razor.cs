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
        /// Gets or sets the list of label groups.
        /// </summary>
        public List<LabelGroup> LabelGroups { get; set; }

        /// <summary>
        /// Gets or sets the ICompendium object.
        /// </summary>
        [Parameter]
        public ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for source selection.
        /// </summary>
        [Parameter]
        public EventCallback<(SourceDataSet, LabelGroup)> SourceSelected { get; set; }

        /// <summary>
        /// Gets or sets the list of temporary labels that have no corresponding SourceDataSets.
        /// </summary>
        [Parameter]
        public List<string> TemporaryLabels { get; set; } = new();

        /// <summary>
        /// Gets the unique list of labels from the SourceDataSets and the TemporaryLabels sorted alphabetically.
        /// </summary>
        /// <returns>
        /// A list of strings representing the unique labels.
        /// </returns>
        public List<string> UniqueLabels
        {
            get
            {
                // Return a list of distinct labels found in the LabelGroups sorted alphabetically.
                return this.LabelGroups.Select(group => group.LabelName).Distinct().OrderBy(label => label).ToList();
            }
        }

        /// <summary>
        /// Gets a list of distinct source data set names from the compendium.
        /// </summary>
        /// <returns>A list of distinct source data set names.</returns>
        private IEnumerable<string> AutoCompleteData
        {
            get
            {
                // Return a list of distinct source data set names from the compendium ordered alphabetically.
                return this.SelectedCompendium.SourceDataSets.Select(sds => sds.SourceDataSetName).Distinct().OrderBy(sds => sds);
            }
        }

        /// <summary>
        /// Gets or sets the filtered compendium.
        /// </summary>
        /// <value>
        /// The filtered compendium.
        /// </value>
        private ICompendium FilteredCompendium { get; set; }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        private string SearchTerm { get; set; } = string.Empty;

        /// <summary>
        /// This method is called when the component receives new parameters. Any initialization or
        /// data fetching logic should be placed here. The StateHasChanged() method is called to
        /// cause the Navigator to re-render.
        /// </summary>
        protected override void OnParametersSet()
        {
            this.SearchTerm = string.Empty;
            this.FilterTree();
        }

        /// <summary>
        /// Overrides the OnParametersSetAsync method to update the label groups.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous operation.
        /// </returns>
        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            this.FilterTree();
            this.UpdateLabelGroups();
        }

        /// <summary>
        /// Filters the tree based on the search term.
        /// </summary>
        private void FilterTree()
        {
            if (string.IsNullOrWhiteSpace(this.SearchTerm))
            {
                this.FilteredCompendium = this.SelectedCompendium;
            }
            else
            {
                this.FilteredCompendium.SourceDataSets =
                    this.SelectedCompendium.SourceDataSets
                    .Where(sds => sds.SourceDataSetName.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            this.StateHasChanged();
        }

        private async Task OnCompendiumExpandedChanged(bool isExpanded, ICompendium compendium)
        {
            await this.CompendiumExpanded.InvokeAsync(compendium);
        }

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
            this.UpdateLabelGroups();
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
            var sourceDataSet = this.FilteredCompendium.SourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(args.Text, StringComparison.OrdinalIgnoreCase));

            // Invoke the SourceSelected event
            await this.SourceSelected.InvokeAsync((sourceDataSet, labelGroup));
        }

        /// <summary>
        /// Updates the label groups based on the source data sets and temporary labels.
        /// </summary>
        private void UpdateLabelGroups()
        {
            // SelectedLabelGroup for SourceDataSets with labels
            var labeledGroupings = this.FilteredCompendium.SourceDataSets
                .SelectMany(ds => ds.Labels.Any() ? ds.Labels.Select(label => new { Label = label, DataSet = ds }) : new[] { new { Label = (string)null, DataSet = ds } })
                .GroupBy(pair => pair.Label)
                .Where(group => !string.IsNullOrEmpty(group.Key))
                .Select(group => new LabelGroup
                {
                    LabelName = group.Key,
                    SourceDataSets = group.Select(pair => pair.DataSet).OrderBy(sds => sds.SourceDataSetName).ToList(),
                });

            // Retrieve the distinct list of labels that are associated with SourceDataSets
            var labelsInUse = labeledGroupings.Select(group => group.LabelName).Distinct().ToList();

            // Remove labels that are in use from the list of temporary labels
            this.TemporaryLabels = this.TemporaryLabels.Except(labelsInUse).ToList();

            // Add the temporary labels to the list of LabelGroups
            labeledGroupings = labeledGroupings.Concat(
                this.TemporaryLabels.Select(label => new LabelGroup
                {
                    LabelName = label,
                    SourceDataSets = new List<SourceDataSet>(),
                }));

            // SelectedLabelGroup for SourceDataSets without labels
            var noLabelGrouping = new LabelGroup
            {
                LabelName = "No Label",
                SourceDataSets = this.FilteredCompendium.SourceDataSets
                    .Where(ds => !ds.Labels.Any())
                    .OrderBy(sds => sds.SourceDataSetName)
                    .ToList(),
            };

            // Merge the two groupings and order them by LabelName
            this.LabelGroups = labeledGroupings.Concat(new[] { noLabelGrouping })
                .OrderBy(group => group.LabelName)
                .ToList();
        }
    }
}
