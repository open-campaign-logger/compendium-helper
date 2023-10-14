// <copyright file="LabelGroup.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Data
{
    using CampaignKit.Compendium.Helper.Configuration;

    /// <summary>
    /// Used for grouping source data sets.
    /// </summary>
    public class LabelGroup
    {
        /// <summary>
        /// Default label to use for LabelGroups objects that have no source data sets.
        /// </summary>
        public static readonly string LabelEmpty = "*No Label";

        /// <summary>
        /// Gets or sets the name of the grouping.
        /// </summary>
        public string LabelName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of source data sets.
        /// </summary>
        /// <returns>The list of source data sets.</returns>
        public List<SourceDataSet> SourceDataSets { get; set; } = new ();

        /// <summary>
        /// Gets the display label for the LabelGroup object.
        /// </summary>
        public string DisplayLabel => $"{this.LabelName} ({this.SourceDataSets.Count})";

        /// <summary>
        /// Overrides the Equals method to compare the SelectedLabelGroup object with another object.
        /// </summary>
        /// <param name="obj">The object to compare with the SelectedLabelGroup object.</param>
        /// <returns>True if the object is a SelectedLabelGroup and has the same LabelName as the SelectedLabelGroup object, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return obj is LabelGroup labelGroup && labelGroup.LabelName == this.LabelName;
        }

        /// <summary>
        /// Overrides the GetHashCode method to return the hash code of the LabelName property.
        /// </summary>
        /// <returns>
        /// The hash code of the LabelName property.
        /// </returns>
        public override int GetHashCode()
        {
            return this.LabelName.GetHashCode();
        }
    }
}
