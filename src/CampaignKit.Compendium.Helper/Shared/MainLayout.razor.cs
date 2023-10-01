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
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Represents the main layout of the application.
    /// </summary>
    public partial class MainLayout    {
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
        /// Gets or sets the JSRuntime for JS interop.
        /// </summary>
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

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
        /// Gets or sets the TooltipService dependency.
        /// </summary>
        [Inject]        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Instantiates a new, blank compendium and assigns it to the SelectedCompendium property.
        /// </summary>
        private void CreateDefaultCompendium()
        {
            this.SelectedCompendium = new PublicCompendium();
        }

        /// <summary>
        /// Downloads a JSON string as a file using JavaScript runtime.
        /// </summary>
        /// <param name="json">The JSON string to download.</param>
        /// <param name="fileName">The name of the file to save the JSON as.</param>
        private void DownloadJSON(string json, string fileName)
        {
            var blob = $"data:text/json;charset=utf-8,{Uri.EscapeDataString(json)}";
            var script = $@"(function() {{
                        var link = document.createElement('a');
                        link.href = '{blob}';
                        link.download = '{fileName}';
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    }})();";
            this.JsRuntime.InvokeVoidAsync("eval", script);
        }

        /// <summary>
        /// Method to handle the event of downloading a selected compendium.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        private async Task OnDownloadSelected()
        {
            if (this.SelectedCompendium == null)
            {
                return;
            }

            var json = this.CompendiumService.SaveCompendium(this.SelectedCompendium);
            this.DownloadJSON(json, this.SelectedCompendium.Title + ".json");
        }

        /// <summary>
        /// Handles the event when a new compendium selection is made by the user.
        /// </summary>
        /// <param name="selection">The selection made by the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnNewCompendiumSelection(bool selection)        {            this.Logger.LogInformation("User selected {Selection}.", selection);            if (selection)            {                this.CreateDefaultCompendium();            }            this.DialogService.Close();        }

        /// <summary>
        /// Method called when the upload of a compendium is complete.
        /// </summary>
        /// <param name="compendium">The uploaded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnUploadComplete(ICompendium compendium)        {            this.SelectedCompendium = compendium;            this.DialogService.Close();        }

        /// <summary>
        /// Shows a new dialog asynchronously and waits for the user's selection. The dialog is opened using the DialogService with the specified title and prompt. The OnSelection event is subscribed to the OnNewCompendiumSelection method. The result of the dialog is displayed as an info notification.
        /// </summary>
        private async void ShowNewDialog()        {
            await this.DialogService.OpenAsync<ConfirmationDialog>(
                "New Compendium",
                new Dictionary<string, object>
                {                    { "Prompt", "Replace the current compendium configuration?" },                    { "OnSelection", EventCallback.Factory.Create<bool>(this, this.OnNewCompendiumSelection) },
                });        }

        /// <summary>
        /// Opens a dialog to confirm loading a package and replacing the current compendium configuration.
        /// </summary>
        /// <param name="packageName">The name of the package to load.</param>
        /// <param name="packageUrl">The URL of the package.</param>
        private async void ShowPackageDialog(string packageName, string packageUrl)        {
            await this.DialogService.OpenAsync<PackageDialog>(
                "Load Package",
                new Dictionary<string, object>
                {                    { "Prompt", $"Load the {packageName} package and replace the current compendium configuration?" },                    { "PackageFileName", packageUrl },                    { "OnUploadComplete", EventCallback.Factory.Create<ICompendium>(this, this.OnUploadComplete) },
                });        }

        /// <summary>
        /// Shows an "Upload File" dialog asynchronously and passes the OnUploadComplete callback method as a parameter.
        /// </summary>
        private async void ShowUploadDialog()        {
            await this.DialogService.OpenAsync<UploadDialog>(                "Upload Compendium",                new Dictionary<string, object>
                {                    { "Prompt", "Select an existing compendium configuration." },                    { "OnUploadComplete", EventCallback.Factory.Create<ICompendium>(this, this.OnUploadComplete) },                });        }
    }}