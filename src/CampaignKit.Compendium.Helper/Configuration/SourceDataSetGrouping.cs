// <copyright file="SourceDataSetGrouping.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Core.Configuration;

    /// <summary>
    /// Used for grouping source data sets.
    /// </summary>
    public class SourceDataSetGrouping
    {
        /// <summary>
        /// Gets or sets the name of the grouping.
        /// </summary>
        public string LabelName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of source data sets.
        /// </summary>
        /// <returns>The list of source data sets.</returns>
        public List<SourceDataSet> SourceDataSets { get; set; } = new List<SourceDataSet>();
    }
}
