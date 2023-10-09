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

namespace CampaignKit.Compendium.Helper.Shared{
    using System.Data;

    using CampaignKit.Compendium.Helper.Configuration;
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
        /// Gets or sets the IJSRuntime dependency.
        /// </summary>
        [Inject]        private IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Gets or sets the JS runtime dependency.
        /// </summary>
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the list of LabelGroup objects.
        /// </summary>
        private List<LabelGroup> LabelGroups { get; set; }

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
        private SourceDataSet SelectedSource { get; set; }

        /// <summary>
        /// Method called when the upload of a compendium is complete.
        /// </summary>
        /// <param name="compendium">The uploaded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CompendiumLoaded(ICompendium compendium)        {            this.Logger.LogInformation("Upload complete: {}", compendium.Title);            this.SelectedCompendium = compendium;
            this.SelectedSource = null;
            this.SelectedLabelGroup = null;
            this.UpdateLabelGroups();        }

        /// <summary>
        /// Instantiates a new, blank compendium and assigns it to the SelectedCompendium property.
        /// </summary>
        private void CreateDefaultCompendium()
        {
            this.Logger.LogInformation("Creating default compendium.");
            this.SelectedCompendium = new Configuration.Compendium();
            this.SelectedSource = null;
            this.SelectedLabelGroup = null;
            this.UpdateLabelGroups();
        }

        /// <summary>
        /// Adds the given list of label groups to the LabelGroups collection and sorts them alphabetically.
        /// </summary>
        /// <param name="labelGroups">The list of label groups to be added.</param>
        private void LabelGroupsAdded(List<LabelGroup> labelGroups)
        {
            // Add the labelGroops to the LabeGroups collection and sort them alphabetically.
            this.LabelGroups.AddRange(labelGroups);
            this.LabelGroups = this.LabelGroups.OrderBy(labelGroup => labelGroup.LabelName).ToList();
        }

        /// <summary>
        /// Removes the given list of label groups to the LabelGroups collection.
        /// </summary>
        /// <param name="labelGroups">The list of label groups to be removed.</param>
        private void LabelGroupsRemoved(List<LabelGroup> labelGroups)
        {
            // Remove the labelGroops to the LabeGroups collection and sort them alphabetically.
            this.LabelGroups.RemoveAll(labelGroup => labelGroups.Contains(labelGroup));
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
                    { "Sources", this.SelectedCompendium.SourceDataSets },
                });
        }

        /// <summary>
        /// Method to handle the event when the user selects to add labelGroups.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnShowAddLabelsDialog()
        {
            this.Logger.LogInformation("User selected to add labelGroups..");

            await this.DialogService.OpenAsync<AddLabelDialog>(
                "Add Labels to Compendium",
                new Dictionary<string, object>
                {
                    { "LabelGroups", this.LabelGroups },
                    { "LabelGroupsAdded", EventCallback.Factory.Create<List<LabelGroup>>(this, this.LabelGroupsAdded) },
                });
        }

        /// <summary>
        /// Opens a dialog to confirm loading a package and replacing the current compendium configuration.
        /// </summary>
        /// <param name="sampleName">The name of the package to load.</param>
        /// <param name="sampleUrl">The URL of the package.</param>
        private async void OnShowLoadDialog(string sampleName, string sampleUrl)        {
            this.Logger.LogInformation("Show compendium load dialog.");

            await this.DialogService.OpenAsync<LoadCompendiumDialog>(
                "Load Sample Compendium",
                new Dictionary<string, object>
                {                    { "Prompt", $"Load the {sampleName} sample compendium?" },                    { "CompendiumUrl", sampleUrl },
                    { "CompendiumLoaded",  EventCallback.Factory.Create<ICompendium>(this, this.CompendiumLoaded) },
                });        }

        /// <summary>
        /// Shows a new dialog asynchronously and waits for the user's selection. The dialog is opened using the DialogService with the specified title and prompt. The OnSelection event is subscribed to the OnNewCompendiumSelection method. The result of the dialog is displayed as an info notification.
        /// </summary>
        private async void OnShowNewDialog()        {
            this.Logger.LogInformation("Show new compendium dialog.");

            await this.DialogService.OpenAsync<ConfirmationDialog>(
                "Create Compendium",
                new Dictionary<string, object>
                {                    { "Prompt", "Replace the current compendium configuration?" },                    { "OnSelection", EventCallback.Factory.Create<bool>(this, this.OnNewCompendiumSelection) },
                });        }

        /// <summary>
        /// Method to handle the event when the user selects to remove labelGroups.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnShowRemoveLabelsDialog()
        {
            this.Logger.LogInformation("User selected to remove labelGroups...");

            await this.DialogService.OpenAsync<RemoveLabelsDialog>(
                "Remove Labels from Compendium",
                new Dictionary<string, object>
                {
                    { "LabelGroups", this.LabelGroups },
                    { "LabelGroupsRemoved", EventCallback.Factory.Create<List<LabelGroup>>(this, this.LabelGroupsRemoved) },
                });
        }

        /// <summary>
        /// Shows an "Upload File" dialog asynchronously and passes the CompendiumLoaded callback method as a parameter.
        /// </summary>
        private async void OnShowUploadDialog()        {
            this.Logger.LogInformation("Show upload dialog.");

            await this.DialogService.OpenAsync<UploadConfigurationDialog>(                "Upload Compendium",                new Dictionary<string, object>
                {                    { "Prompt", "Select an existing compendium configuration." },                    { "CompendiumLoaded",  EventCallback.Factory.Create<ICompendium>(this, this.CompendiumLoaded) },                });        }

        /// <summary>
        /// Event handler for when the selected compendium changes.
        /// </summary>
        /// <param name="compendium">The new selected compendium.</param>
        private async Task SelectedCompendiumChanged(ICompendium compendium)
        {
            await this.UpdatePageTitle();
        }

        /// <summary>
        /// Event handler for when the selected label group changes.
        /// </summary>
        /// <param name="labelGroup">The new selected label group.</param>
        private void SelectedLabelGroupChanged(LabelGroup labelGroup)
        {
            this.SelectedLabelGroup = labelGroup;
        }

        /// <summary>
        /// Event handler for when the selected source data set changes.
        /// </summary>
        /// <param name="sourceDataSet">The new selected source data set.</param>
        private void SelectedSourceDataSetChanged(SourceDataSet sourceDataSet)
        {
            this.SelectedSource = sourceDataSet;
        }

        /// <summary>
        /// Updates the label groups based on the source data sets and temporary labelGroups.
        /// </summary>
        private void UpdateLabelGroups()
        {
            // Create LabelGroups for Labels in use by Sources
            this.LabelGroups = this.SelectedCompendium.SourceDataSets
                .SelectMany(
                    ds => ds.Labels.Any()
                        ? ds.Labels.Select(label => new { Label = label, DataSet = ds })
                        : new[] { new { Label = "*No Label", DataSet = new SourceDataSet() } })
                .GroupBy(pair => pair.Label)
                .Select(group => new LabelGroup
                {
                    LabelName = group.Key,
                    SourceDataSets = group.Select(pair => pair.DataSet).OrderBy(sds => sds.SourceDataSetName).ToList(),
                })
                .ToList();
        }

        /// <summary>
        /// Updates the title of the browser window with the title of the selected compendium, or sets it to "Compendium Helper" if no compendium is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task UpdatePageTitle()        {            var title = string.IsNullOrEmpty(this.SelectedCompendium?.Title) ? "Compendium Helper" : this.SelectedCompendium.Title;            await this.BrowserService.SetTitle(this.JsRuntime, title);        }
    }}