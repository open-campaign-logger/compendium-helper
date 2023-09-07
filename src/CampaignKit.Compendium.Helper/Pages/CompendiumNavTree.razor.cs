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

    using Microsoft.AspNetCore.Components;

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
        /// Gets or sets the filtered compendiums.
        /// </summary>
        /// <value>
        /// The filtered compendiums.
        /// </value>
        private IEnumerable<PublicCompendium> FilteredCompendiums { get; set; }

        /// <summary>
        /// Gets or sets the property to store the search term.
        /// </summary>
        private string SearchTerm { get; set; } = string.Empty;

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            this.FilterTree();
        }

        /// <summary>
        /// This method is called when the component receives new parameters. Any initialization or
        /// data fetching logic should be placed here. The StateHasChanged() method is called to
        /// cause the CompendiumNavTree to re-render.
        /// </summary>
        protected override void OnParametersSet()
        {
            this.FilterTree();
        }

        /// <summary>
        /// Filters the tree based on the search term.
        /// </summary>
        private void FilterTree()
        {
            if (string.IsNullOrWhiteSpace(this.SearchTerm))
            {
                this.FilteredCompendiums = this.PublicCompendiums;
            }
            else
            {
                this.FilteredCompendiums = this.PublicCompendiums
                    .Where(pc => pc.SourceDataSets.Any(sds => sds.SourceDataSetName.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(pc => new PublicCompendium
                    {
                        // Clone or copy other properties if needed
                        // Example:
                        // CompendiumService = pc.CompendiumService,
                        CompendiumService = pc.CompendiumService,
                        Description = pc.Description,
                        GameSystem = pc.GameSystem,
                        ImageUrl = pc.ImageUrl,
                        IsActive = pc.IsActive,
                        OverwriteExisting = pc.OverwriteExisting,
                        Prompts = pc.Prompts,
                        Title = pc.Title,
                        SourceDataSets = pc.SourceDataSets
                            .Where(sds => sds.SourceDataSetName.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase))
                            .ToList(),
                    })
                    .ToList();
            }

            this.StateHasChanged();
        }
    }
}
