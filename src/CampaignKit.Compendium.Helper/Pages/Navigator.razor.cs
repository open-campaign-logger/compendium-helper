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

    using CampaignKit.Compendium.Core.Configuration;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Represents a partial class for the Navigator component.
    /// </summary>
    public partial class Navigator
    {
        /// <summary>
        /// Gets or sets the ICompendium object.
        /// </summary>
        [Parameter]
        public ICompendium Compendium { get; set; }

        /// <summary>
        /// Gets a list of distinct source data set names from the compendium.
        /// </summary>
        /// <returns>A list of distinct source data set names.</returns>
        private IEnumerable<string> AutoCompleteData
        {
            get
            {
                return this.Compendium.SourceDataSetGroupings
                        .SelectMany(s => s.SourceDataSets)
                        .Select(ds => ds.SourceDataSetName)
                        .Distinct();
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
        /// Filters the tree based on the search term.
        /// </summary>
        private void FilterTree()
        {
            if (string.IsNullOrWhiteSpace(this.SearchTerm))
            {
                this.FilteredCompendium = this.Compendium;
            }
            else
            {
                this.FilteredCompendium.SourceDataSets =
                    this.Compendium.SourceDataSets
                    .Where(sds => sds.SourceDataSetName.Contains(this.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            this.StateHasChanged();
        }

        /// <summary>
        /// Handles the search input change event and filters the tree accordingly.
        /// </summary>
        private void OnSearchChanged(object value)
        {
            this.FilterTree();
        }
    }
}
