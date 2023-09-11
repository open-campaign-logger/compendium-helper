// <copyright file="LabelProperties.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using System.Linq;
    using System.Threading.Tasks;

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
        public List<SourceDataSet> AllSourceDataSets { get; set; }

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
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<LabelProperties> Logger { get; set; }

        /// <summary>
        /// Gets or sets the values of the selected data sets.
        /// </summary>
        private IEnumerable<string> SelectedDataSets { get; set; }

        /// <summary>
        /// Sorts the source data sets and gets the list of selected data sets.
        /// </summary>
        /// <returns>
        /// A list of strings containing the selected data sets.
        /// </returns>
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Sort the source data sets.
            this.AllSourceDataSets.Sort((x, y) => x.SourceDataSetName.CompareTo(y.SourceDataSetName));

            // Sort the source data sets associated with the grouping.
            this.SourceDataSetGrouping.SourceDataSets.Sort((x, y) => x.SourceDataSetName.CompareTo(y.SourceDataSetName));

            // Get the list of selected data sets, convert them to a list of strings, sort them and then assign them to SelectedDataSets.
            this.SelectedDataSets
                = this.SourceDataSetGrouping.SourceDataSets.Select(x => x.SourceDataSetName.ToString()).OrderBy(x => x);
        }

        /// <summary>
        /// Logs a message when the source data set association is changed.
        /// </summary>
        /// <param name="selected">The source data sets.</param>
        private void OnChange(object selected)
        {
            // Log the method entry
            this.Logger.LogInformation("Source data set association changed.");

            // Case selected to List<string> to simplify working with it.
            var selectedSourceDataSets = (IEnumerable<string>) selected;
            var toBeRemoved = new List<string>();
            var toBeAdded = new List<string>();

            // Iterate through SourceDataSetGrouping.SourceDataSets to see if any of them have been removed.  selected is a List<string>
            foreach (var sourceDataSet in this.SourceDataSetGrouping.SourceDataSets)
            {
                if (!selectedSourceDataSets.Contains(sourceDataSet.SourceDataSetName))
                {
                    // Add the sourceDataSetName to the toBeRemoved list.
                    toBeRemoved.Add(sourceDataSet.SourceDataSetName);
                }
            }

            // Cycle through toBeRemoved list and remove the appropriate sourcedatasets from the SourceDataSetGrouping.SourceDataSets list.
            foreach (var tbr in toBeRemoved)
            {
                // Find the object in the SourceDataSetGrouping.SourceDataSets lists
                var sourceDataSet = this.SourceDataSetGrouping.SourceDataSets.First(x => x.SourceDataSetName.Equals(tbr));

                // Remove the label from the sourceDataSet.
                sourceDataSet.Labels.Remove(this.SourceDataSetGrouping.LabelName);

                // Remove the label from the source data set grouping.
                this.SourceDataSetGrouping.SourceDataSets.Remove(sourceDataSet);
            }

            // Iterate through selected to see if any new labels have been added.
            foreach (var sourceDataSetName in selectedSourceDataSets)
            {
                // If the sourceDataSetName is not in the SourceDataSetGrouping.SourceDataSets list, add it.
                if (!this.SourceDataSetGrouping.SourceDataSets.Any(x => x.SourceDataSetName == sourceDataSetName))
                {
                    // Get the SourceDataSet object from AllSourceDataSets.
                    var sourceDataSet = this.AllSourceDataSets.FirstOrDefault(x => x.SourceDataSetName == sourceDataSetName);

                    // Add the label to the sourceDataSet.
                    sourceDataSet.Labels.Add(this.SourceDataSetGrouping.LabelName);

                    // Add the sourceDataSet to the SourceDataSetGrouping.SourceDataSets list.
                    this.SourceDataSetGrouping.SourceDataSets.Add(sourceDataSet);
                }
            }
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
    }
}
