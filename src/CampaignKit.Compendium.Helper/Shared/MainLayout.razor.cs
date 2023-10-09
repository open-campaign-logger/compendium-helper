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
        /// Gets or sets the list of temporary labels.
        /// </summary>
        private List<string> TemporaryLabels { get; set; } = new ();

        /// <summary>
        /// Instantiates a new, blank compendium and assigns it to the SelectedCompendium property.
        /// </summary>
        private void CreateDefaultCompendium()
        {
            this.Logger.LogInformation("Creating default compendium.");
            this.SelectedCompendium = new Configuration.Compendium();
            this.SelectedSource = null;
            this.SelectedLabelGroup = null;
            this.TemporaryLabels = new List<string>();
            this.UpdateLabelGroups();
        }

        /// <summary>
        /// Updates the label groups based on the source data sets and temporary labels.
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
                .Concat(this.TemporaryLabels.Select(label => new LabelGroup
                {
                    LabelName = label,
                    SourceDataSets = new List<SourceDataSet>(),
                }))
                .OrderBy(group => group.LabelName)
                .ToList();
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
        /// Adds the new list of labels to the TemporaryLabels property of the SelectedCompendium if they are not already present in the UniqueLabels property.
        /// </summary>
        /// <param name="labels">The list of labels to be added.</param>
        private void OnLabelsAdded(List<string> labels)
        {
            // Retrieve a list of labels being referenced by a SourceDataSet in the SelectedCompendium
            var uniqueLabels = this.SelectedCompendium.SourceDataSets.SelectMany(sds => sds.Labels).Distinct().ToList();

            // Trim all labels and remove any empty labels
            labels = labels.Select(label => label.Trim()).Where(label => !string.IsNullOrEmpty(label)).ToList();

            // Add labels to TemporaryLabels if they are not already in the collection
            this.TemporaryLabels = this.TemporaryLabels.Union(labels).ToList();

            // Remove any labels from TemporaryLabels that are already in the UniqueLabels list
            this.TemporaryLabels = this.TemporaryLabels.Except(uniqueLabels).ToList();
        }

        /// <summary>
        /// Removes the specified labels from any Sources referencing them and from the TemporaryLabels collection.
        /// </summary>
        /// <param name="labels">The labels to be removed.</param>
        private void OnLabelsRemoved(List<string> labels)
        {
            // Remove these labels from any Sources referencing them.
            foreach (var label in labels)
            {
                // Find all Sources that contain the label and remove it.
                this.SelectedCompendium.SourceDataSets.ForEach(sds => sds.Labels.Remove(label));
            }

            // Remove these labels from the TemporaryLabels collection.
            this.TemporaryLabels = this.TemporaryLabels.Except(labels).ToList();
        }

        /// <summary>
        /// Handles the event when a new compendium selection is made by the user.
        /// </summary>
        /// <param name="selection">The selection made by the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnNewCompendiumSelection(bool selection)        {            this.Logger.LogInformation("User selected to create a new compendium.");            if (selection)            {                this.CreateDefaultCompendium();            }        }

        /// <summary>
        /// Method to handle the event when the user selects to remove labels.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnRemoveLabels()
        {
            this.Logger.LogInformation("User selected to remove labels...");

            // Retrieve a list of labels being referenced by a SourceDataSet in the SelectedCompendium
            var labelsInUse = this.SelectedCompendium.SourceDataSets.SelectMany(sds => sds.Labels).Distinct().ToList();

            // Add labels to TemporaryLabels if they are not already in the collection
            var allLabels = this.TemporaryLabels.Union(labelsInUse).OrderBy(label => label).ToList();

            await this.DialogService.OpenAsync<RemoveLabelsDialog>(
                "Remove Labels from Compendium",
                new Dictionary<string, object>
                {
                    { "AllLabels", allLabels },
                });
        }

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
            this.SelectedSource = sourceDataSet;
        }

        /// <summary>
        /// Method called when the upload of a compendium is complete.
        /// </summary>
        /// <param name="compendium">The uploaded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnUploadComplete(ICompendium compendium)        {            this.Logger.LogInformation("Upload complete: {}", compendium.Title);            this.SelectedCompendium = compendium;
            this.SelectedSource = null;
            this.SelectedLabelGroup = null;
            this.TemporaryLabels = new List<string>();
            this.UpdateLabelGroups();        }

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