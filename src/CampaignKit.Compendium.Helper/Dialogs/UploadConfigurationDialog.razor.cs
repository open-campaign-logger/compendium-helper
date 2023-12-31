﻿// <copyright file="UploadConfigurationDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Dialogs{    using CampaignKit.Compendium.Helper.Configuration;    using CampaignKit.Compendium.Helper.Services;    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Represents a dialog for uploading files.
    /// </summary>
    public partial class UploadConfigurationDialog    {
        /// <summary>
        /// Gets or sets the event callback for when the upload is complete.
        /// The event callback takes an ICompendium parameter.
        /// </summary>
        [Parameter]        public EventCallback<ICompendium> CompendiumLoaded { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the parameter.
        /// </summary>
        /// <value>The prompt.</value>
        [Parameter]        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets injects the CompendiumService dependency into the CompendiumService property.
        /// </summary>
        [Inject]        private CompendiumService CompendiumService { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets injects an ILogger dependency into the Logger property.
        /// </summary>
        [Inject]        private ILogger<UploadConfigurationDialog> Logger { get; set; }

        /// <summary>
        /// Gets or sets the progress of the download.
        /// </summary>
        private double UploadProgress { get; set; }

        private void OnProgress(UploadProgressArgs args)
        {
            this.UploadProgress = args.Progress;
        }

        /// <summary>
        /// Method called when the upload is complete.
        /// </summary>
        /// <param name="args">The event arguments containing the raw response.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnCompendiumLoaded(UploadCompleteEventArgs args)        {            this.Logger.LogInformation("Upload complete and converted to string of length {Length}.", args.RawResponse.Length);            var json = args.RawResponse;            var compendium = this.CompendiumService.LoadCompendium(json);            await this.CompendiumLoaded.InvokeAsync(compendium);            this.DialogService.Close();        }    }}