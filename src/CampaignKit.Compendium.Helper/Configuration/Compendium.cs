// <copyright file="Compendium.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Configuration
{
    using System.Data;

    using CampaignKit.Compendium.Helper.Data;

    using Newtonsoft.Json;

    /// <summary>
    /// Represents the configuration of an open-source compendium within the application.
    /// These should be defined and shared in the appsettings.json file.
    /// </summary>
    public class Compendium : ICompendium
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
        public List<Prompt> Prompts { get; set; } = new() { };

        /// <inheritdoc/>
        public List<SourceDataSet> SourceDataSets { get; set; } = new();

        /// <inheritdoc/>
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonIgnore]
        public List<string> UniqueLabels
        {
            get
            {

                // Return a unique list of labels from the SourceDataSets and the TemporaryLabels sorted alphabetically.
                return this.SourceDataSets
                    .SelectMany(ds => ds.Labels)
                    .Concat(this.TemporaryLabels)
                    .Distinct()
                    .OrderBy(label => label)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets or sets the list of temporary labels that have no corresponding SourceDataSets.
        /// </summary>
        [JsonIgnore]
        public List<string> TemporaryLabels { get; set; } = new ();

        /// <inheritdoc/>
        [JsonIgnore]
        public List<LabelGroup> LabelGroups
        {
            get
            {
                // LabelGroup for SourceDataSets with labels
                var labeledGroupings = this.SourceDataSets
                    .SelectMany(ds => ds.Labels.Any() ? ds.Labels.Select(label => new { Label = label, DataSet = ds }) : new[] { new { Label = (string)null, DataSet = ds } })
                    .GroupBy(pair => pair.Label)
                    .Where(group => !string.IsNullOrEmpty(group.Key))
                    .Select(group => new LabelGroup
                    {
                        LabelName = group.Key,
                        SourceDataSets = group.Select(pair => pair.DataSet).OrderBy(sds => sds.SourceDataSetName).ToList(),
                    });

                // Retrieve the distinct list of labels that are associated with SourceDataSets
                var labelsInUse = labeledGroupings.Select(group => group.LabelName).Distinct().ToList();

                // Remove labels that are in use from the list of temporary labels
                this.TemporaryLabels = this.TemporaryLabels.Except(labelsInUse).ToList();

                // Add the temporary labels to the list of LabelGroups
                labeledGroupings = labeledGroupings.Concat(
                    this.TemporaryLabels.Select(label => new LabelGroup
                    {
                        LabelName = label,
                        SourceDataSets = new List<SourceDataSet>(),
                    }));

                // LabelGroup for SourceDataSets without labels
                var noLabelGrouping = new LabelGroup
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
