// <copyright file="MainLayout.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Shared{    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Data;
    using CampaignKit.Compendium.Helper.Dialogs;
    using CampaignKit.Compendium.Helper.Pages;
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Represents the main layout of the application.
    /// </summary>
    public partial class MainLayout    {
        /// <summary>
        /// Gets or sets the BrowserService dependency.
        /// </summary>
        [Inject]
        private BrowserService BrowserService { get; set; }

        /// <summary>
        /// Gets or sets the Compendium dependency.
        /// </summary>
        [Inject]
        private CompendiumService CompendiumService { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the JS runtime dependency.
        /// </summary>
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the ILogger into the Logger property.
        /// </summary>
        [Inject]        private ILogger<MainLayout> Logger { get; set; }

        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        /// <value>The selected compendium.</value>
        private ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the property to store the selected label.
        /// </summary>
        private LabelGroup SelectedLabelGroup { get; set; }

        /// <summary>
        /// Gets or sets the selected source data set.
        /// </summary>
        private SourceDataSet SelectedSourceDataSet { get; set; }

        /// <summary>
        /// Instantiates a new, blank compendium and assigns it to the SelectedCompendium property.
        /// </summary>
        private void CreateDefaultCompendium()
        {
            this.Logger.LogInformation("Creating default compendium.");
            this.SelectedCompendium = new Configuration.Compendium();
        }

        /// <summary>
        /// Adds the new list of labels to the TemporaryLabels property of the SelectedCompendium if they are not already present in the UniqueLabels property.
        /// </summary>
        /// <param name="labels">The list of labels to be added.</param>
        private void OnLabelsAdded(List<string> labels)
        {
        }

        /// <summary>
        /// Method to handle the event when the user selects to add labels.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnAddLables()
        {
            this.Logger.LogInformation("User selected to add labels..");

            await this.DialogService.OpenAsync<AddLabelDialog>(
                "Add Labels to Compendium",
                new Dictionary<string, object>
                {
                    { "OnLabelsAdded", EventCallback.Factory.Create<List<string>>(this, this.OnLabelsAdded) },
                });
        }

        /// <summary>
        /// Method to handle the event when the user selects to add sources.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnAddSources()
        {
            this.Logger.LogInformation("User selected to add sources..");

            await this.DialogService.OpenAsync<AddSourcesDialog>(
                "Add Sources to Compendium",
                new Dictionary<string, object>                {
                    { "Compendium", this.SelectedCompendium },
                });
        }

        /// <summary>
        /// Method to handle the event of downloading a selected compendium.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnDownload()
        {
            this.Logger.LogInformation("User selected to download the current compendium.");

            if (this.SelectedCompendium == null)
            {
                return;
            }

            var json = this.CompendiumService.SaveCompendium(this.SelectedCompendium);
            await this.BrowserService.DownloadTextFile(this.JSRuntime, json, "compendium-helper.json");
        }

        /// <summary>
        /// Method to handle the generation of a campaign JSON file based on the selected compendium.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnGenerate()
        {
            this.Logger.LogInformation("User selected to generate a campaign JSON file.");

            if (this.SelectedCompendium == null)
            {
                return;
            }

            await this.DialogService.OpenAsync<Generator>(
                "Generate Campaign Logger File",
                new Dictionary<string, object>                {
                    { "SelectedCompendium", this.SelectedCompendium },
                });
        }

        /// <summary>
        /// Handles the event when a new compendium selection is made by the user.
        /// </summary>
        /// <param name="selection">The selection made by the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnNewCompendiumSelection(bool selection)        {            this.Logger.LogInformation("User selected to create a new compendium.");            if (selection)            {                this.CreateDefaultCompendium();            }        }

        /// <summary>
        /// Method to handle the event when the user selects to remove sources.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnRemoveSources()
        {
            this.Logger.LogInformation("User selected to remove sources..");

            await this.DialogService.OpenAsync<RemoveSourcesDialog>(
                "Remove Sources from Compendium",
                new Dictionary<string, object>
                {
                    { "AllSources", this.SelectedCompendium.SourceDataSets },
                });
        }

        /// <summary>
        /// Event handler for when the selected compendium changes.
        /// </summary>
        /// <param name="compendium">The new selected compendium.</param>
        private void OnSelectedCompendiumChanged(ICompendium compendium)
        {
            this.SelectedCompendium = compendium;
        }

        /// <summary>
        /// Event handler for when the selected label group changes.
        /// </summary>
        /// <param name="labelGroup">The new selected label group.</param>
        private void OnSelectedLabelGroupChanged(LabelGroup labelGroup)
        {
            this.SelectedLabelGroup = labelGroup;
        }

        /// <summary>
        /// Event handler for when the selected source data set changes.
        /// </summary>
        /// <param name="sourceDataSet">The new selected source data set.</param>
        private void OnSelectedSourceDataSetChanged(SourceDataSet sourceDataSet)
        {
            this.SelectedSourceDataSet = sourceDataSet;
        }

        /// <summary>
        /// Method called when the upload of a compendium is complete.
        /// </summary>
        /// <param name="compendium">The uploaded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnUploadComplete(ICompendium compendium)        {            this.Logger.LogInformation("Upload complete: {}", compendium.Title);            this.SelectedCompendium = compendium;        }

        /// <summary>
        /// Shows a new dialog asynchronously and waits for the user's selection. The dialog is opened using the DialogService with the specified title and prompt. The OnSelection event is subscribed to the OnNewCompendiumSelection method. The result of the dialog is displayed as an info notification.
        /// </summary>
        private async void ShowNewDialog()        {
            this.Logger.LogInformation("Show new compendium dialog.");

            await this.DialogService.OpenAsync<ConfirmationDialog>(
                "Create Compendium",
                new Dictionary<string, object>
                {                    { "Prompt", "Replace the current compendium configuration?" },                    { "OnSelection", EventCallback.Factory.Create<bool>(this, this.OnNewCompendiumSelection) },
                });        }

        /// <summary>
        /// Opens a dialog to confirm loading a package and replacing the current compendium configuration.
        /// </summary>
        /// <param name="sampleName">The name of the package to load.</param>
        /// <param name="sampleUrl">The URL of the package.</param>
        private async void ShowPackageDialog(string sampleName, string sampleUrl)        {
            this.Logger.LogInformation("Show package dialog.");

            await this.DialogService.OpenAsync<LoadConfigurationDialog>(
                "Load Sample Configuration",
                new Dictionary<string, object>
                {                    { "Prompt", $"Load the {sampleName} sample configuration?" },                    { "SampleConfigurationName", sampleUrl },
                    { "OnUploadComplete",  EventCallback.Factory.Create<ICompendium>(this, this.OnUploadComplete) },
                });        }

        /// <summary>
        /// Shows an "Upload File" dialog asynchronously and passes the OnUploadComplete callback method as a parameter.
        /// </summary>
        private async void ShowUploadDialog()        {
            this.Logger.LogInformation("Show upload dialog.");

            await this.DialogService.OpenAsync<UploadConfigurationDialog>(                "Upload Compendium",                new Dictionary<string, object>
                {                    { "Prompt", "Select an existing compendium configuration." },                    { "OnUploadComplete",  EventCallback.Factory.Create<ICompendium>(this, this.OnUploadComplete) },                });        }
    }}