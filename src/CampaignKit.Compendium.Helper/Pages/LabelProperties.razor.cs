﻿// <copyright file="LabelProperties.razor.cs" company="Jochen Linnemann - IT-Service">
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
    /// Partial class for the LabelProperties component.
    /// </summary>
    public partial class LabelProperties
    {
        /// <summary>
        /// Gets or sets the list of all available SourceDataSet objects.
        /// </summary>
        [Parameter]
        public List<SourceDataSet> AllSourceDataSetGroupings { get; set; }

        /// <summary>
        /// Gets or sets the SourceDataSetGrouping parameter.
        /// </summary>
        [Parameter]
        public SourceDataSetGrouping SourceDataSetGrouping { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        public TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets a list of all data sets that are not assigned to any group.
        /// </summary>
        private List<SourceDataSet> UnassignedDataSets
        {
            get
            {
                return this.AllSourceDataSetGroupings.Except(this.SourceDataSetGrouping.SourceDataSets).ToList();
            }
        }

        /// <summary>
        /// Assigns a given SourceDataSet to the SourceDataSetGrouping.
        /// </summary>
        /// <param name="dataSet">The SourceDataSet to be assigned.</param>
        private void AssignDataSet(SourceDataSet dataSet)
        {
            this.SourceDataSetGrouping.SourceDataSets.Add(dataSet);
        }

        /// <summary>
        /// Opens a tooltip with the specified content for the given element.
        /// </summary>
        /// <param name="elementReference">The element to open the tooltip for.</param>
        /// <param name="tooltip">The tooltip content.</param>
        /// <param name="options">Optional options for the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)
        {
            this.TooltipService.Open(elementReference, tooltip, options);
        }

        /// <summary>
        /// Removes the specified SourceDataSet from the SourceDataSetGrouping.
        /// </summary>
        /// <param name="dataSet">The SourceDataSet to remove.</param>
        private void UnassignDataSet(SourceDataSet dataSet)
        {
            this.SourceDataSetGrouping.SourceDataSets.Remove(dataSet);
        }
    }
}
