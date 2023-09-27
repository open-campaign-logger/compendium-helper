// <copyright file="Index.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using Core.Configuration;
    using Data;
    using Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Code behind class for Index.razor.
    /// </summary>
    public partial class Index
    {
        /// <summary>
        /// Gets or sets the CompendiumService.
        /// </summary>
        [Inject]
        private CompendiumService CompendiumService { get; set; }

        /// <summary>
        /// Gets or sets the JSRuntime for JS interop.
        /// </summary>
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<Index> Logger { get; set; }

        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        private ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the selected source data set.
        /// </summary>
        private SourceDataSet SelectedSourceDataSet { get; set; }

        /// <summary>
        /// Gets or sets the property to store the selected label.
        /// </summary>
        private LabelGroup SelectedSourceDataSetGrouping { get; set; }

        /// <summary>
        /// Handles the compendium collapsed event from the navigator component.
        /// </summary>
        /// <param name="compendiumName">The name of the selected compendium.</param>
        private void OnCompendiumCollapsed(string compendiumName)
        {
            Logger.LogInformation("SelectedCompendium collapsed: {CompendiumName}", compendiumName);

            // Update user selections
            SelectedSourceDataSetGrouping = null;
            SelectedSourceDataSet = null;
        }

        /// <summary>
        /// Handles the compendium expansion event from the navigator component.
        /// </summary>
        /// <param name="compendiumName">The name of the selected compendium.</param>
        private void OnCompendiumExpanded(string compendiumName)
        {
            Logger.LogInformation("SelectedCompendium expanded: {CompendiumName}", compendiumName);

            // Update user selections
            SelectedSourceDataSetGrouping = null;
            SelectedSourceDataSet = null;
        }

        /// <summary>
        /// Logs a message when the label assignment is changed and triggers a state change.
        /// </summary>
        private void OnLabelAssignmentChanted(string labelName)
        {
            Logger.LogInformation("Label assignment changed: {LabelName}", labelName);
            StateHasChanged();
        }

        /// <summary>
        /// Handles the label collapse event from the navigator component.
        /// </summary>
        /// <param name="labelName">The name of the label that was selected.</param>
        private void OnLabelCollapsed(string labelName)
        {
            Logger.LogInformation("Label collapsed: {LabelName}", labelName);

            // Update user selections
            SelectedSourceDataSetGrouping
                = SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(labelName), null);
            SelectedSourceDataSet = null;
        }

        /// <summary>
        /// Handles the label expansion event from the navigator component.
        /// </summary>
        /// <param name="labelName">The name of the label that was selected.</param>
        private void OnLabelExpanded(string labelName)
        {
            Logger.LogInformation("Label expanded: {LabelName}", labelName);

            // Update user selections
            SelectedSourceDataSetGrouping
                = SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(labelName), null);
            SelectedSourceDataSet = null;
        }

        /// <summary>
        /// Handles the SelectedSource clicked event by logging the selected SelectedSource name.
        /// </summary>
        /// <param name="sourceDataSetName">Name of the SelectedSource.</param>
        private async Task OnSourceDataSetSelected(string sourceDataSetName)
        {
            Logger.LogInformation("Selected SelectedSource: {SourceDataSetName}", sourceDataSetName);

            // Update user selections
            SelectedSourceDataSetGrouping = null;
            SelectedSourceDataSet
                = SelectedCompendium.SourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(sourceDataSetName), null);
        }

        /// <summary>
        /// Uploads a compendium and creates a tree from it.
        /// </summary>
        /// <param name="args">The upload event arguments.</param>
        /// <returns>
        /// A tree created from the uploaded compendium.
        /// </returns>
        private async Task UploadComplete(UploadCompleteEventArgs args)
        {
            Logger.LogInformation("Upload complete and converted to string of length {Length}.", args.RawResponse.Length);
            var json = args.RawResponse;
            SelectedCompendium = CompendiumService.LoadCompendiums(json);
            SelectedSourceDataSet = null;
        }
    }
}