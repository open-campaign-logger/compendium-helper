// <copyright file="Label.razor.cs" company="Jochen Linnemann - IT-Service">
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

    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Data;
    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Partial class for the Label component.
    /// </summary>
    public partial class Label
    {
        /// <summary>
        /// Gets or sets the list of all available SourceDataSet objects.
        /// </summary>
        [Parameter]
        public List<SourceDataSet> AllSources { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for the label assignment change event.
        /// </summary>
        [Parameter]
        public EventCallback<string> LabelChanged { get; set; }

        /// <summary>
        /// Gets or sets the SelectedSource parameter.
        /// </summary>
        [Parameter]
        public LabelGroup SelectedSource { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        public TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<Label> Logger { get; set; }

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
        protected override async Task OnParametersSetAsync()
        {
            this.Logger.LogInformation("OnParametersSetAsync");
            await base.OnParametersSetAsync();

            // Sort the source data sets.
            this.AllSources.Sort((x, y) => string.Compare(x.SourceDataSetName, y.SourceDataSetName, StringComparison.InvariantCultureIgnoreCase));

            // Sort the source data sets associated with the grouping.
            this.SelectedSource.SourceDataSets.Sort((x, y) => string.Compare(x.SourceDataSetName, y.SourceDataSetName, StringComparison.InvariantCultureIgnoreCase));

            // Get the list of selected data sets, convert them to a list of strings, sort them and then assign them to SelectedDataSets.
            this.SelectedDataSets
                = this.SelectedSource.SourceDataSets.Select(x => x.SourceDataSetName.ToString()).OrderBy(x => x);
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
            var selectedSourceDataSets = (IEnumerable<string>)selected;
            var toBeAdded = new List<string>();

            // Iterate through SelectedSource.SourceDataSets to see if any of them have been removed.  selected is a List<string>
            var toBeRemoved = (from sourceDataSet in this.SelectedSource.SourceDataSets where !selectedSourceDataSets.Contains(sourceDataSet.SourceDataSetName) select sourceDataSet.SourceDataSetName).ToList();

            // Cycle through toBeRemoved list and remove the appropriate sourcedatasets from the SelectedSource.SourceDataSets list.
            foreach (var tbr in toBeRemoved)
            {
                // Find the object in the SelectedSource.SourceDataSets lists
                var sourceDataSet = this.SelectedSource.SourceDataSets.First(x => x.SourceDataSetName.Equals(tbr));

                // Remove the label from the sourceDataSet.
                sourceDataSet.Labels.Remove(this.SelectedSource.LabelName);

                // Remove the label from the source data set grouping.
                this.SelectedSource.SourceDataSets.Remove(sourceDataSet);
            }

            // Iterate through selected to see if any new labels have been added.
            foreach (var sourceDataSetName in selectedSourceDataSets)
            {
                // If the sourceDataSetName is not in the SelectedSource.SourceDataSets list, add it.
                {
                    // Get the SourceDataSet object from AllSources.
                    var sourceDataSet = this.AllSources.FirstOrDefault(x => x.SourceDataSetName == sourceDataSetName);

                    // Add the label to the sourceDataSet.
                    if (sourceDataSet != null)
                    {
                        sourceDataSet.Labels.Add(this.SelectedSource.LabelName);

                        // Add the sourceDataSet to the SelectedSource.SourceDataSets list.
                        this.SelectedSource.SourceDataSets.Add(sourceDataSet);
                    }
                }
            }

            // Fire an event to notify the parent component that the label assignment has changed.
            this.LabelChanged.InvokeAsync(this.SelectedSource.LabelName);
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
