// <copyright file="Helper.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using System;

    using CampaignKit.Compendium.Core.Configuration;
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Code behind class for Helper.razor.
    /// </summary>
    public partial class Helper
    {
        /// <summary>
        /// Gets or sets the CompendiumService.
        /// </summary>
        [Inject]
        private CompendiumService CompendiumService { get; set; }

        /// <summary>
        /// Gets or sets the DownloadService.
        /// </summary>
        [Inject]
        private DownloadService DownloadService { get; set; }

        /// <summary>
        /// Gets or sets the source HTML for the current data set.
        /// </summary>
        private string Html { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the HtmlService.
        /// </summary>
        [Inject]
        private HtmlService HtmlService { get; set; }

        /// <summary>
        /// Gets or sets the JSRuntime for JS interop.
        /// </summary>
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<Helper> Logger { get; set; }

        /// <summary>
        /// Gets or sets the Markdown conversion of the extracted HTML.
        /// </summary>
        private string Markdown { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MarkdownService.
        /// </summary>
        [Inject]
        private MarkdownService MarkdownService { get; set; }

        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        private ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the selected source data set.
        /// </summary>
        private SourceDataSet SelectedSourceDataSet { get; set; }

        /// <summary>
        /// Handles the SourceDataSet clicked event by logging the selected SourceDataSet name.
        /// </summary>
        /// <param name="sourceDataSetName">Name of the SourceDataSet.</param>
        private void HandleSourceDataSetClicked(string sourceDataSetName)
        {
            this.Logger.LogInformation("Selected SourceDataSet: {SourceDataSetName}", sourceDataSetName);
            this.SelectedSourceDataSet
                = this.SelectedCompendium.SourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(sourceDataSetName), null);
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
            this.Logger.LogInformation("Upload complete and converted to string of length {Length}.", args.RawResponse.Length);
            var json = args.RawResponse;
            this.SelectedCompendium = this.CompendiumService.LoadCompendiums(json);
            this.SelectedSourceDataSet = null;
        }
    }
}