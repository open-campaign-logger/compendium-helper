// <copyright file="SourceDataSetService.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Configuration;

    using HtmlAgilityPack;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Service for loading source data sets.
    /// </summary>
    public class SourceDataSetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDataSetService"/> class.
        /// </summary>
        /// <param name="downloadService">BrowserService DI service.</param>
        /// <param name="logger">ILogger DI service.</param>
        /// <param name="markdownService">MarkdownService DI service.</param>
        /// <returns>
        /// SourceDataSetService object.
        /// </returns>
        public SourceDataSetService(
            FetchService downloadService,
            ILogger<SourceDataSetService> logger,
            MarkdownService markdownService)
        {
            this.DownloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.MarkdownService = markdownService ?? throw new ArgumentNullException(nameof(markdownService));
        }

        /// <summary>
        /// Gets or sets the BrowserService.
        /// </summary>
        [Inject]
        private FetchService DownloadService { get; set; }

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
        /// Loads the response data set and set the HTML and Markdown properties.
        /// </summary>
        /// <param name="source">The source data set to load.</param>
        /// <param name="forceReload">A value indicating whether to force a reload of the source data set.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task LoadSourceDataSetAsync(SourceDataSet source, bool forceReload = false)
        {
            this.Logger.LogInformation("Loading source data set: {SelectedSource}, Force reload: {Force}", source, forceReload);

            // If the source data set already has a Markdown property, then it has already been loaded.
            if (source == null || (!string.IsNullOrEmpty(source.Markdown) && !forceReload))
            {
                return;
            }
            else
            {
                // Otherwise, download the HTML from the source's SourceDataSetURI property.
                var response = await this.DownloadService.GetWebPageAync(source.SourceDataSetUri);

                // If the source's XPath is not null or empty navigate to the starting XPath denoted by the SourceDataSetXPath property using the HtmlAgilityPack.
                if (!string.IsNullOrEmpty(source.XPath))
                {
                    // Load the HTML into an HtmlAgilityPack HtmlDocument object.
                    var doc = new HtmlDocument();
                    doc.LoadHtml(response);

                    // Navigate to the starting XPath denoted by the SourceDataSetXPath property using the HtmlAgilityPack.
                    try
                    {
                        var node = doc.DocumentNode.SelectSingleNode(source.XPath);
                        response = node.OuterHtml;
                    }
                    catch
                    {
                        response = $"Unable to find node corresponding to XPath: {source.XPath}";
                    }
                }

                // Convert to markdown
                source.Markdown = this.MarkdownService.ConvertHtmlToMarkdown(response);
            }
        }
    }
}
