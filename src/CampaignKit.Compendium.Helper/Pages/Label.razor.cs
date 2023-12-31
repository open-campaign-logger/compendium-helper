﻿// <copyright file="Label.razor.cs" company="Jochen Linnemann - IT-Service">
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
        public List<SourceDataSet> Sources { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for the label group change event.
        /// </summary>
        [Parameter]
        public EventCallback<LabelGroup> SelectedLabelGroupChanged { get; set; }

        /// <summary>
        /// Gets or sets the SelectedLabelGroup parameter.
        /// </summary>
        [Parameter]
        public LabelGroup SelectedLabelGroup { get; set; }

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
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Sorts the source data sets and gets the list of selected data sets.
        /// </summary>
        /// <returns>
        /// A list of strings containing the selected data sets.
        /// </returns>
        protected override async Task OnParametersSetAsync()
        {
            this.Logger.LogInformation("OnParametersSetAsync");
            if (this.SelectedLabelGroup != null && this.SelectedLabelGroup.SourceDataSets != null)
            {
                this.SelectedDataSets
                    = this.SelectedLabelGroup.SourceDataSets.Select(x => x.SourceDataSetName.ToString()).OrderBy(x => x);
            }
        }

        /// <summary>
        /// Logs a message when the source data set association is changed.
        /// </summary>
        /// <param name="selected">The source data sets.</param>
        private void OnSelectedLabelGroupChanged(object selected)
        {
            // Log the method entry
            this.Logger.LogInformation("Label association changed.");

            // Case selected to List<string> to simplify working with it.
            List<string> selectedSourceDataSets = ((IEnumerable<string>)selected).ToList();

            // Iterate through SelectedLabelGroup.Sources to see if any of them have been removed.  selected is a List<string>
            List<string> toBeRemoved = (from sourceDataSet in this.SelectedLabelGroup.SourceDataSets where !selectedSourceDataSets.Contains(sourceDataSet.SourceDataSetName) select sourceDataSet.SourceDataSetName).ToList();

            // Cycle through toBeRemoved list and remove the appropriate sourcedatasets from the SelectedLabelGroup.Sources list.
            foreach (string tbr in toBeRemoved)
            {
                // Find the object in the SelectedLabelGroup.Sources lists
                SourceDataSet sourceDataSet = this.SelectedLabelGroup.SourceDataSets.First(x => x.SourceDataSetName.Equals(tbr));

                // Remove the label from the sourceDataSet.
                sourceDataSet.Labels.Remove(this.SelectedLabelGroup.LabelName);

                // Remove the source data set from the SelectedLabelGroup
                this.SelectedLabelGroup.SourceDataSets.Remove(sourceDataSet);
            }

            // Iterate through selected to see if any new labels have been added.
            foreach (string sourceDataSetName in selectedSourceDataSets)
            {
                // If the sourceDataSetName is not in the SelectedLabelGroup.Sources list, add it.
                {
                    // Get the SourceDataSet object from Sources.
                    SourceDataSet sourceDataSet = this.Sources.First(x => x.SourceDataSetName == sourceDataSetName);

                    // Add the label to the sourceDataSet if it doesn't already exist.
                    if (!sourceDataSet.Labels.Contains(this.SelectedLabelGroup.LabelName))
                    {
                        sourceDataSet.Labels.Add(this.SelectedLabelGroup.LabelName);

                        // Add the sourceDataSet to the SelectedLabelGroup.Sources list.
                        this.SelectedLabelGroup.SourceDataSets.Add(sourceDataSet);
                    }
                }
            }

            // Sort the SelectedLabelGroup.Sources list.
            this.SelectedLabelGroup.SourceDataSets = this.SelectedLabelGroup.SourceDataSets.OrderBy(x => x.SourceDataSetName).ToList();

            // Fire an event to notify the parent component that the label assignment has changed.
            this.SelectedLabelGroupChanged.InvokeAsync(this.SelectedLabelGroup);
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
