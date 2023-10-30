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
        /// Gets or sets the selected source data set.
        /// </summary>
        private SourceDataSet SelectedSource { get; set; }

        /// <summary>
        /// Method called when the upload of a compendium is complete.
        /// </summary>
        /// <param name="compendium">The uploaded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CompendiumLoaded(ICompendium compendium)        {            this.Logger.LogInformation("Compendium load complete: {}", compendium.Title);            this.SelectedCompendium = compendium;
            this.SelectedSource = null;
            this.UpdateLabelGroups();        }

        /// <summary>
        /// Instantiates a new, blank compendium and assigns it to the SelectedCompendium property.
        /// </summary>
        private void CreateDefaultCompendium()
        {
            this.Logger.LogInformation("Creating default compendium.");
            this.SelectedCompendium = new Configuration.Compendium();
            this.SelectedSource = null;
            this.UpdateLabelGroups();
        }

        /// <summary>
        /// Method to handle the event of downloading a selected compendium.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnDownload()
        {
            this.Logger.LogInformation("Downloading the current compendium.");

            if (this.SelectedCompendium == null)
            {
                return;
            }

            var json = this.CompendiumService.SaveCompendium(this.SelectedCompendium);
            await this.BrowserService.DownloadTextFile(this.JsRuntime, json, "compendium-helper.json");
        }

        /// <summary>
        /// Method to handle the generation of a campaign JSON file based on the selected compendium.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnGenerate()
        {
            this.Logger.LogInformation("Showing Generator Dialog...");

            if (this.SelectedCompendium == null)
            {
                return;
            }

            await this.DialogService.OpenAsync<GeneratorDialog>(
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
        private async Task OnNewCompendiumSelection(bool selection)        {            this.Logger.LogInformation("Creating a new compendium.");            if (selection)            {                this.CreateDefaultCompendium();            }        }

        /// <summary>
        /// Method to handle the event when the user adds a source.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnAddSource()
        {
            this.Logger.LogInformation("Adding Source...");

            // Determine the next available SourceDataSet name.
            var sourceDataSetNumber = 1;

            var sourceDataSetName = "Compendium Source";
            while (this.SelectedCompendium.SourceDataSets.Any(sds => sds.SourceDataSetName.Equals($"{sourceDataSetName} {sourceDataSetNumber}")))
            {
                sourceDataSetNumber++;
            }

            // Create a SourceDataSet object.
            this.SelectedSource = new SourceDataSet()
            {
                SourceDataSetName = $"{sourceDataSetName} {sourceDataSetNumber}",
                SourceDataSetUri = string.Empty,
                Labels = new List<string>(),
                TagSymbol = "~",
                IsPublic = true,
            };

            // Add the new source to the collection.
            this.SelectedCompendium.SourceDataSets.Add(this.SelectedSource);

            this.SelectedCompendium.SourceDataSets = this.SelectedCompendium.SourceDataSets.OrderBy(source => source.SourceDataSetName).ToList();
            this.UpdateLabelGroups();
        }

        /// <summary>
        /// Method to handle the event when the user clones a source.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnCloneSource()
        {
            this.Logger.LogInformation("Cloning Source...");

            if (this.SelectedSource == null)
            {
                return;
            }

            // Determine the next available SourceDataSet name.
            var sourceDataSetNumber = 1;

            var sourceDataSetName = this.SelectedSource.SourceDataSetName;
            while (this.SelectedCompendium.SourceDataSets.Any(sds => sds.SourceDataSetName.Equals($"{sourceDataSetName} {sourceDataSetNumber}")))
            {
                sourceDataSetNumber++;
            }

            // Create a SourceDataSet object.
            this.SelectedSource = new SourceDataSet()
            {
                SourceDataSetName = $"{sourceDataSetName} {sourceDataSetNumber}",
                SourceDataSetUri = this.SelectedSource.SourceDataSetUri,
                Labels = this.SelectedSource.Labels,
                TagSymbol = this.SelectedSource.TagSymbol,
                IsPublic = this.SelectedSource.IsPublic,
                XPath = this.SelectedSource.XPath,
                Markdown = this.SelectedSource.Markdown,
            };

            // Add the new source to the collection.
            this.SelectedCompendium.SourceDataSets.Add(this.SelectedSource);

            this.SelectedCompendium.SourceDataSets = this.SelectedCompendium.SourceDataSets.OrderBy(source => source.SourceDataSetName).ToList();
            this.UpdateLabelGroups();
        }

        /// <summary>
        /// Opens a dialog to confirm loading a package and replacing the current compendium configuration.
        /// </summary>
        /// <param name="sampleName">The name of the package to load.</param>
        /// <param name="sampleUrl">The URL of the package.</param>
        private async void OnShowLoadDialog(string sampleName, string sampleUrl)        {
            this.Logger.LogInformation("Showing Load Compendium Dialog...");

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
            this.Logger.LogInformation("Showing New Compendium Dialog...");

            await this.DialogService.OpenAsync<ConfirmationDialog>(
                "Create Compendium",
                new Dictionary<string, object>
                {                    { "Prompt", "Replace the current compendium configuration?" },                    { "OnSelection", EventCallback.Factory.Create<bool>(this, this.OnNewCompendiumSelection) },
                });        }

        /// <summary>
        /// Method to handle the event when the user selects to remove a source.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnRemoveSource()
        {
            this.Logger.LogInformation("Removing Source...");

            if (this.SelectedSource != null)
            {
                this.SelectedCompendium.SourceDataSets.Remove(this.SelectedSource);
                this.SelectedSource = null;
                this.UpdateLabelGroups();
            }
        }

        /// <summary>
        /// Shows an "Upload File" dialog asynchronously and passes the CompendiumLoaded callback method as a parameter.
        /// </summary>
        private async void OnShowUploadDialog()        {
            this.Logger.LogInformation("Showing Compendium Upload Dialog.");

            await this.DialogService.OpenAsync<UploadConfigurationDialog>(                "Upload Compendium",                new Dictionary<string, object>
                {                    { "Prompt", "Select an existing compendium configuration." },                    { "CompendiumLoaded",  EventCallback.Factory.Create<ICompendium>(this, this.CompendiumLoaded) },                });        }

        /// <summary>
        /// Event handler for when the selected compendium changes.
        /// </summary>
        /// <param name="compendium">The new selected compendium.</param>
        private async Task SelectedCompendiumChanged(ICompendium compendium)
        {
            this.Logger.LogInformation("SelectedCompendiumChanged: {}", compendium?.Title ?? "Null");
            await this.UpdatePageTitle();
        }

        /// <summary>
        /// Updates the selected source data set if it is different from the current selected source.
        /// </summary>
        /// <param name="sourceDataSet">The new source data set to be selected.</param>
        private void SelectedSourceChanged(SourceDataSet sourceDataSet)
        {
            this.Logger.LogInformation("SelectedSourceChanged: {}", sourceDataSet?.SourceDataSetName ?? "Null");
            if (sourceDataSet != null)
            {
                this.SelectedSource = sourceDataSet;
                this.UpdateLabelGroups();
            }
        }

        /// <summary>
        /// Updates the label groups based on the source data sets and temporary labelGroups.
        /// </summary>
        private void UpdateLabelGroups()
        {
            this.Logger.LogInformation("Updating Label Groups");

            // Create LabelGroups for Labels in use by Sources
            this.LabelGroups = this.SelectedCompendium.SourceDataSets
                .SelectMany(
                    ds => ds.Labels.Any()
                        ? ds.Labels.Select(label => new { Label = label, DataSet = ds })
                        : new[] { new { Label = LabelGroup.LabelEmpty, DataSet = ds } })
                .GroupBy(pair => pair.Label)
                .Select(group => new LabelGroup
                {
                    LabelName = group.Key,
                    SourceDataSets = group.Select(pair => pair.DataSet).OrderBy(sds => sds.SourceDataSetName).ToList(),
                })
                .OrderBy(group => group.LabelName)
                .ToList();

            // Create a List<string> that contains an entry for each of this.LabelGroup and is of the format "LabelName (SourceDataSets.Count)"
            this.Logger.LogDebug("Updated label groups: {}", string.Join(", ", this.LabelGroups.Select(labelGroup => labelGroup.LabelName)));
        }

        /// <summary>
        /// Updates the title of the browser window with the title of the selected compendium, or sets it to "Compendium Helper" if no compendium is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task UpdatePageTitle()        {            this.Logger.LogInformation("Updating browser page title: {}", this.SelectedCompendium?.Title ?? "Compendium Helper");            var title = string.IsNullOrEmpty(this.SelectedCompendium?.Title) ? "Compendium Helper" : this.SelectedCompendium.Title;            await this.BrowserService.SetTitle(this.JsRuntime, title);        }
    }}