// <copyright file="CampaignLoggerService.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.CampaignLogger;
    using CampaignKit.Compendium.Helper.Configuration;

    using Newtonsoft.Json;

    /// <summary>
    /// Campaign Logger dependency injection service.
    /// </summary>
    public class CampaignLoggerService
    {
        /// <summary>
        /// Gets or sets the ILogger into the Logger property.
        /// </summary>
        private readonly ILogger<CampaignLoggerService> logger;

        /// <summary>
        /// Gets or sets the SourceDataSetService into the SourceDataSetService property.
        /// </summary>
        private readonly SourceDataSetService sourceDataSetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignLoggerService"/> class.
        /// </summary>
        /// <param name="logger">Logger object for logging.</param>
        /// <param name="sourceDataSetService">Service for working with source data sets.</param>
        /// <returns>
        /// BrowserService object.
        /// </returns>
        public CampaignLoggerService(
            ILogger<CampaignLoggerService> logger,
            SourceDataSetService sourceDataSetService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sourceDataSetService = sourceDataSetService ?? throw new ArgumentNullException(nameof(sourceDataSetService));
        }

        /// <summary>
        /// Event that is raised when the progress changes.
        /// </summary>
        public event EventHandler<int> ProgressChanged;

        /// <summary>
        /// Event that is raised when the status has changed.
        /// </summary>
        /// <remarks>
        /// The event handler receives a string parameter containing the new status.
        /// </remarks>
        public event EventHandler<string> StatusChanged;

        /// <summary>
        /// Converts the given compendium to a campaign object and serializes it to JSON format.
        /// </summary>
        /// <param name="compendium">The compendium to convert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The JSON representation of the converted campaign.</returns>
        public async Task<string> ConvertToCampaignJson(ICompendium compendium, CancellationToken cancellationToken)
        {
            this.StatusChanged?.Invoke(this, $"Converting compendium to campaign: {compendium.Title}");
            this.ProgressChanged?.Invoke(this, 0);
            var campaign = await this.ConvertToCampaign(compendium, cancellationToken);
            var json = JsonConvert.SerializeObject(campaign, Formatting.Indented);
            return json;
        }

        /// <summary>
        /// Converts a given Compendium object to a Campaign object.
        /// </summary>
        /// <param name="compendium">The Compendium object to convert.</param>
        /// <param name="cancellationToken">The cancelation token.</param>
        /// <returns>The converted Campaign object.</returns>
        private async Task<Campaign> ConvertToCampaign(ICompendium compendium, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Converting compendium to campaign: {Compendium}", compendium.Title);

            var campaignEntries = new List<CampaignEntry>();

            double progressPerEntry = 100.0 / compendium.SourceDataSets.Count;
            double progress = 0;

            foreach (var source in compendium.SourceDataSets)
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.StatusChanged?.Invoke(this, $"Converting source data set to campaign entry: {source.SourceDataSetName}");
                var campaignEntry = await this.ConvertToCampaignEntry(source);
                if (string.IsNullOrEmpty(campaignEntry.RawPublic) || string.IsNullOrEmpty(campaignEntry.RawText))
                {
                    this.StatusChanged?.Invoke(this, $"Unable to retrieve data for source data set: {source.SourceDataSetName}.");
                    continue;
                }

                campaignEntries.Add(campaignEntry);
                progress += progressPerEntry;
                this.ProgressChanged?.Invoke(this, (int)progress);
            }

            var campaign = new Campaign()
            {
                Version = 2,
                Type = "campaign",
                Title = compendium.Title,
                Description = compendium.Description,
                CampaignEntries = campaignEntries,
                Logs = new List<Log>(),
                ImageUrl = compendium.ImageUrl,
            };

            return campaign;
        }

        /// <summary>
        /// Converts a SourceDataSet object to a CampaignEntry object.
        /// </summary>
        /// <param name="source">The SourceDataSet object to convert.</param>
        /// <returns>The converted CampaignEntry object.</returns>
        private async Task<CampaignEntry> ConvertToCampaignEntry(SourceDataSet source)
        {
            this.logger.LogInformation("Converting source data set to campaign entry: {SourceDataSet}", source.SourceDataSetName);

            // Load the source data set.
            await this.sourceDataSetService.LoadSourceDataSetAsync(source);

            // Convert the source data set to a campaign entry.
            CampaignEntry campaignEntry = new ()
            {
                RawText = source.IsPublic ? string.Empty : source.Markdown,
                RawPublic = source.IsPublic ? source.Markdown : string.Empty,
                Labels = source.Labels,
                TagSymbol = source.TagSymbol,
                TagValue = source.SourceDataSetName,
            };

            return campaignEntry;
        }
    }
}
