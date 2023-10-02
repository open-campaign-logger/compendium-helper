// <copyright file="Generator.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Represents a dialog for logging messages.
    /// </summary>
    public partial class Generator
    {
        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        /// <value>The selected compendium.</value>
        [Parameter]
        public ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the BrowserService dependency.
        /// </summary>
        [Inject]
        private BrowserService BrowserService { get; set; }

        /// <summary>
        /// Gets or sets the CampaignLoggerService dependency.
        /// </summary>
        [Inject]
        private CampaignLoggerService CampaignLoggerService { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]
        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the IJSRuntime dependency.
        /// </summary>
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the ILogger dependency.
        /// </summary>
        [Inject]
        private ILogger<Generator> Logger { get; set; }

        /// <summary>
        /// Gets or sets the progress value.
        /// </summary>
        /// <value>The progress value.</value>
        private int Progress { get; set; }

        /// <summary>
        /// Gets or sets the status parameter.
        /// </summary>
        /// <value>The status.</value>
        private string Status { get; set; }

        /// <summary>
        /// Initializes the component by subscribing to the ProgressChanged and StatusChanged events
        /// of the CampaignLoggerService, and setting the GenerationProgress to 0 and
        /// GenerationStatus to an empty string.
        /// </summary>
        protected override void OnInitialized()
        {
            this.Logger.LogInformation("Initializing Generator component.");
            this.CampaignLoggerService.ProgressChanged += this.OnProgressChanged;
            this.CampaignLoggerService.StatusChanged += this.OnStatusChanged;
            this.Progress = 0;
            this.Status = string.Empty;
        }

        /// <summary>
        /// Method to handle the cancel event.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnCancel()
        {
            this.Logger.LogInformation("User selected to cancel the current generation.");
            this.DialogService.Close();
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
            this.Progress = 0;
            this.Status = string.Empty;
            var json = await this.CampaignLoggerService.ConvertToCampaignJson(this.SelectedCompendium);
            await this.BrowserService.DownloadTextFile(this.JSRuntime, json, "compendium-helper.json");
            this.DialogService.Close();
        }

        /// <summary>
        /// Event handler for the progress changed event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The progress value.</param>
        private void OnProgressChanged(object sender, int e)
        {
            this.Progress = e;
            this.StateHasChanged();
        }

        /// <summary>
        /// Event handler for the status changed event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="s">The status message.</param>
        private void OnStatusChanged(object sender, string s)
        {
            this.Status += Environment.NewLine + s;
            this.StateHasChanged();
        }
    }
}