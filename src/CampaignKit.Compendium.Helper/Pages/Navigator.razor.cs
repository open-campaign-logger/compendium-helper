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
    using CampaignKit.Compendium.Core.Configuration;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;

    using Radzen;
    using Radzen.Blazor;

    using System;

    using static System.Runtime.InteropServices.JavaScript.JSType;

    /// <summary>
    /// Represents a partial class for the Navigator component.
    /// </summary>
    public partial class Navigator
    {
        /// <summary>
        /// Gets or sets the list of PublicCompendium objects.
        /// </summary>
        [Parameter]
        public List<PublicCompendium> PublicCompendiums { get; set; }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; } = string.Empty;

        /// <summary>
        /// Gets a list of distinct source data set names from the public compendiums.
        /// </summary>
        /// <returns>A list of distinct source data set names.</returns>
        private IEnumerable<string> AutoCompleteData
        {
            get
            {
                return this.PublicCompendiums
                        .SelectMany(c => c.SourceDataSetGroupings)
                        .SelectMany(s => s.SourceDataSets)
                        .Select(ds => ds.SourceDataSetName)
                        .Distinct();
            }
        }

        /// <summary>
        /// Gets or sets the RadzenContextMenu object.
        /// </summary>
        private RadzenContextMenu ContextMenu { get; set; }

        /// <summary>
        /// Gets or sets the ContextMenuService.
        /// </summary>
        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        void ShowContextMenuWithItems(MouseEventArgs args)
        {
            ContextMenuService.Open(args,
                new List<ContextMenuItem> {
                new ContextMenuItem(){ Text = "Context menu item 1", Value = 1, Icon = "home" },
                new ContextMenuItem(){ Text = "Context menu item 2", Value = 2, Icon = "search" },
                new ContextMenuItem(){ Text = "Context menu item 3", Value = 3, Icon = "info" },
             }, OnMenuItemClick);
        }

        void OnMenuItemClick(MenuItemEventArgs args)
        {
            // console.Log($"Menu item with Value={args.Value} clicked");
            if (!args.Value.Equals(3) && !args.Value.Equals(4))
            {
                ContextMenuService.Close();
            }
        }


        /// <summary>
        /// Gets or sets the filtered compendiums.
        /// </summary>
        /// <value>
        /// The filtered compendiums.
        /// </value>
        private IEnumerable<PublicCompendium> FilteredCompendiums { get; set; }

        /// <summary>
        /// This method is called when the component receives new parameters. Any initialization or
        /// data fetching logic should be placed here. The StateHasChanged() method is called to
        /// cause the Navigator to re-render.
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

        /// <summary>
        /// Handles the search input change event and filters the tree accordingly.
        /// </summary>
        private void OnSearchChanged(object value)
        {
            this.FilterTree();
        }
    }
}
