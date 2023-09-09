// <copyright file="PublicCompendium.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Core.Configuration
{
    using System.Data;

    using CampaignKit.Compendium.Helper.Configuration;

    /// <summary>
    /// Represents the configuration of an open-source compendium within the application.
    /// These should be defined and shared in the appsettings.json file.
    /// </summary>
    public class PublicCompendium : ICompendium
    {
        /// <inheritdoc/>
        public string CompendiumService { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string GameSystem { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string ImageUrl { get; set; } = string.Empty;

        /// <inheritdoc/>
        public bool IsActive { get; set; } = true;

        /// <inheritdoc/>
        public bool OverwriteExisting { get; set; } = false;

        /// <inheritdoc/>
        public List<Prompt> Prompts { get; set; } = new List<Prompt> { };

        /// <inheritdoc/>
        public List<SourceDataSet> SourceDataSets { get; set; } = new List<SourceDataSet>();

        /// <inheritdoc/>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets a list of unique labels from all source datasets.
        /// </summary>
        /// <returns>A list of unique labels.</returns>
        public List<string> UniqueLabels
        {
            get
            {
                return this.SourceDataSets
                    .SelectMany(ds => ds.Labels)
                    .Distinct()
                    .OrderBy(label => label)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a list of SourceDataSetGroupings from the SourceDataSets.
        /// </summary>
        /// <returns>A list of SourceDataSetGroupings.</returns>
        public List<SourceDataSetGrouping> SourceDataSetGroupings
        {
            get
            {
                // Grouping for SourceDataSets with labels
                var labeledGroupings = this.SourceDataSets
                    .SelectMany(ds => ds.Labels.Any() ? ds.Labels.Select(label => new { Label = label, DataSet = ds }) : new[] { new { Label = (string)null, DataSet = ds } })
                    .GroupBy(pair => pair.Label)
                    .Where(group => !string.IsNullOrEmpty(group.Key))
                    .Select(group => new SourceDataSetGrouping
                    {
                        LabelName = group.Key,
                        SourceDataSets = group.Select(pair => pair.DataSet).OrderBy(sds => sds.SourceDataSetName).ToList(),
                    });

                // Grouping for SourceDataSets without labels
                var noLabelGrouping = new SourceDataSetGrouping
                {
                    LabelName = "No Label",
                    SourceDataSets = this.SourceDataSets
                        .Where(ds => !ds.Labels.Any())
                        .OrderBy(sds => sds.SourceDataSetName)
                        .ToList(),
                };

                // Merge the two groupings and order them by LabelName
                return labeledGroupings.Concat(new[] { noLabelGrouping })
                    .OrderBy(group => group.LabelName)
                    .ToList();
            }
        }
    }
}
