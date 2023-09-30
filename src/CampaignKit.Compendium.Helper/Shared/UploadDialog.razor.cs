﻿// <copyright file="UploadDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Shared

    using Radzen;

    /// <summary>
    /// Represents a dialog for uploading files.
    /// </summary>
    public partial class UploadDialog
        /// <summary>
        /// Gets or sets the event callback for when the upload is complete.
        /// The event callback takes an ICompendium parameter.
        /// </summary>
        [Parameter]

        /// <summary>
        /// Gets or sets the prompt for the parameter.
        /// </summary>
        /// <value>The prompt.</value>
        [Parameter]

        /// <summary>
        /// Gets or sets injects the CompendiumService dependency into the CompendiumService property.
        /// </summary>
        [Inject]

        /// <summary>
        /// Gets or sets injects an ILogger dependency into the Logger property.
        /// </summary>
        [Inject]

        /// <summary>
        /// Gets or sets the progress of the download.
        /// </summary>
        private double DownloadProgress { get; set; }

        private void OnProgress(UploadProgressArgs args)
        {
            this.DownloadProgress = args.Progress;
        }

        /// <summary>
        /// Method called when the upload is complete.
        /// </summary>
        /// <param name="args">The event arguments containing the raw response.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UploadComplete(UploadCompleteEventArgs args)