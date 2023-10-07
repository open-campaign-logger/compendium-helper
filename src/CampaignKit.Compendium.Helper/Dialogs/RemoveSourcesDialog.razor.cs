// <copyright file="RemoveSourcesDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Dialogs
{
    using CampaignKit.Compendium.Helper.Configuration;

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Code behind for the RemoveSourcesDialog component.
    /// </summary>
    public partial class RemoveSourcesDialog
    {
        /// <summary>
        /// Gets or sets the list of all available SourceDataSet objects.
        /// </summary>
        [Parameter]
        public List<SourceDataSet> AllSources { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]
        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets a value indicating whether the SelectedDataSets property is not null or empty.
        /// </summary>
        /// <returns>
        /// Returns true if SelectedDataSets is not null or empty, otherwise false.
        /// </returns>
        private bool IsValid
        {
            get
            {
                // return true if SelectedDataSets is not null or empty.
                return this.SelectedDataSets != null && this.SelectedDataSets.Any();
            }
        }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<RemoveSourcesDialog> Logger { get; set; }

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
        }

        /// <summary>
        /// Removes the selected data sets from the list of all sources and raises the SourceRemoved event.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnRemove()
        {
            // Remove the selected data sets from the list of all sources.
            this.AllSources.RemoveAll(x => this.SelectedDataSets.Contains(x.SourceDataSetName));

            // Close the dialog box.
            this.DialogService.Close();
        }
    }
}
