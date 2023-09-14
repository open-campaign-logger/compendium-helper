﻿// <copyright file="SourceDataSetService.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Services
{
    using CampaignKit.Compendium.Core.Configuration;
    using CampaignKit.Compendium.Helper.Pages;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Service for loading source data sets.
    /// </summary>
    public class SourceDataSetService
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDataSetService"/> class.
        /// </summary>
        /// <param name="downloadService">DownloadService DI service.</param>
        /// <param name="htmlService">HtmlService DI service.</param>
        /// <param name="logger">ILogger DI service.</param>
        /// <param name="markdownService">MarkdownService DI service.</param>
        /// <returns>
        /// SourceDataSetService object.
        /// </returns>
        public SourceDataSetService(
            DownloadService downloadService,
            HtmlService htmlService,
            ILogger<SourceDataSetService> logger,
            MarkdownService markdownService)
        {
            this.DownloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
            this.HtmlService = htmlService ?? throw new ArgumentNullException(nameof(htmlService));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.MarkdownService = markdownService ?? throw new ArgumentNullException(nameof(markdownService));
        }

        /// <summary>
        /// Gets or sets the DownloadService.
        /// </summary>
        [Inject]
        private DownloadService DownloadService { get; set; }

        /// <summary>
        /// Gets or sets the HTMLService.
        /// </summary>
        [Inject]
        private HtmlService HtmlService { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<SourceDataSetService> Logger { get; set; }

        /// <summary>
        /// Gets or sets the MarkdownService.
        /// </summary>
        [Inject]
        private MarkdownService MarkdownService { get; set; }

        /// <summary>
        /// Loads the html data set and set the HTML and Markdown properties.
        /// </summary>
        /// <param name="source">The source data set to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task LoadSourceDataSetAsync(SourceDataSet source)
        {
            this.Logger.LogInformation("Loading source data set: {Source}", source);
            try
            {
                var html = await this.DownloadService.GetWebPageAync(source.SourceDataSetURI);
                if (html != null)
                {
                    source.HTML = html;
                    source.Markdown = this.MarkdownService.ConvertHtmlToMarkdown(html);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error loading html data set.");
                source.HTML = "Unable to load source data.";
                source.Markdown = "Unable to load source data.";
            }
        }
    }
}