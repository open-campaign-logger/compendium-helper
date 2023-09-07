// <copyright file="CompendiumNavTree.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Core.Configuration;
    using CampaignKit.Compendium.Helper.Configuration;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Represents a partial class for the CompendiumNavTree component.
    /// </summary>
    public partial class CompendiumNavTree
    {
        /// <summary>
        /// Gets or sets the list of PublicCompendium objects.
        /// </summary>
        [Parameter]
        public List<PublicCompendium> PublicCompendiums { get; set; }

        /// <summary>
        /// Gets or sets the set of expanded nodes.
        /// </summary>
        private HashSet<string> ExpandedNodes { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the filtered compendiums.
        /// </summary>
        /// <value>
        /// The filtered compendiums.
        /// </value>
        private IEnumerable<PublicCompendium> FilteredCompendiums { get; set; }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            this.FilterTree(string.Empty);
        }

        /// <summary>
        /// This method is called when the component receives new parameters. Any initialization or
        /// data fetching logic should be placed here. The StateHasChanged() method is called to
        /// cause the CompendiumNavTree to re-render.
        /// </summary>
        protected override void OnParametersSet()
        {
            this.ExpandedNodes.Clear();
            this.FilterTree(string.Empty);
        }

        /// <summary>
        /// Filters the tree based on the search term.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        private void FilterTree(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                this.FilteredCompendiums = this.PublicCompendiums;
            }
            else
            {
                this.FilteredCompendiums = this.PublicCompendiums
                    .Where(pc => pc.SourceDataSets.Any(sds => sds.SourceDataSetName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(pc => new PublicCompendium
                    {
                        CompendiumService = pc.CompendiumService,
                        Description = pc.Description,
                        GameSystem = pc.GameSystem,
                        ImageUrl = pc.ImageUrl,
                        IsActive = pc.IsActive,
                        OverwriteExisting = pc.OverwriteExisting,
                        Prompts = pc.Prompts,
                        Title = pc.Title,
                        SourceDataSets = pc.SourceDataSets
                            .Where(sds => sds.SourceDataSetName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                            .ToList(),
                    })
                    .ToList();
            }

            this.StateHasChanged();
        }

        /// <summary>
        /// Checks if the given node is expanded.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node is expanded, false otherwise.</returns>
        private bool IsNodeExpanded(object node)
        {
            if (node != null && node is SourceDataSetGrouping sourceDataSetGrouping)
            {
                return this.ExpandedNodes.Contains(sourceDataSetGrouping.LabelName);
            }

            return false;
        }


        private void OnExpand(TreeExpandEventArgs args)
        {
            if (args.Value is SourceDataSetGrouping sourceDataSetGrouping)
            {
                this.ExpandedNodes.Add(sourceDataSetGrouping.LabelName);
            }
        }

        private void OnCollapse(TreeEventArgs args)
        {
            if (args.Value is SourceDataSetGrouping sourceDataSetGrouping)
            {
                this.ExpandedNodes.Remove(sourceDataSetGrouping.LabelName);
            }
        }

        /// <summary>
        /// Handles the search input change event and filters the tree accordingly.
        /// </summary>
        private void OnSearchChanged(ChangeEventArgs e)
        {
            this.FilterTree(e.Value.ToString());
        }
    }
}
