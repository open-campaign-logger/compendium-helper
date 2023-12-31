﻿// <copyright file="HtmlService.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Data;

    using Markdig;

    /// <summary>
    /// This class provides methods for manipulating HTML strings.
    /// </summary>
    public class HtmlService
    {
        private readonly ILogger<HtmlService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlService"/> class.
        /// </summary>
        /// <param name="logger">Logger object for logging.</param>
        /// <returns>
        /// HtmlService object.
        /// </returns>
        public HtmlService(ILogger<HtmlService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Converts a Markdown string to HTML using Markdig.
        /// </summary>
        /// <param name="markdown">The Markdown string to convert.</param>
        /// <returns>The HTML string.</returns>
        public string ConvertMarkdownToHtml(string markdown)
        {
            // Validate parameters
            if (markdown == null)
            {
                throw new ArgumentNullException(nameof(markdown));
            }

            // Log method entry.
            this.logger.LogInformation("ConvertMarkdownToHtml method called with markdown: {Markdown}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(markdown, 50));

            // Use Markdig to convert Markdown to HTML
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var response = Markdown.ToHtml(markdown, pipeline);

            // Log the response
            this.logger.LogInformation("ConvertMarkdownToHtml method completed with response: {Response}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(markdown, 50));

            // Return the response.
            return response;
        }
    }
}