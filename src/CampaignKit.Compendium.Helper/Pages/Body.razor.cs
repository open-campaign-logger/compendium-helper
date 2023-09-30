// <copyright file="Body.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Data;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Code behind class for Body.razor.
    /// </summary>
    public partial class Body
    {

        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        [Parameter]
        public ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<Body> Logger { get; set; }

        /// <summary>
        /// Gets or sets the selected tab index.
        /// </summary>
        private int SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the selected source data set.
        /// </summary>
        private SourceDataSet SelectedSourceDataSet { get; set; }

        /// <summary>
        /// Gets or sets the property to store the selected label.
        /// </summary>
        private LabelGroup SelectedSourceDataSetGrouping { get; set; }

        /// <summary>
        /// This method is called when the component's parameters are set. It first calls the base implementation of the method. Then, it sets the SelectedSourceDataSet and SelectedSourceDataSetGrouping properties to null.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            this.SelectedSourceDataSet = null;
            this.SelectedSourceDataSetGrouping = null;
            this.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the compendium collapsed event from the navigator component.
        /// </summary>
        /// <param name="compendiumName">The name of the selected compendium.</param>
        private void OnCompendiumCollapsed(string compendiumName)
        {
            this.Logger.LogInformation("SelectedCompendium collapsed: {CompendiumName}", compendiumName);

            // Update user selections
            this.SelectedSourceDataSetGrouping = null;
            this.SelectedSourceDataSet = null;
            this.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the compendium expansion event from the navigator component.
        /// </summary>
        /// <param name="compendiumName">The name of the selected compendium.</param>
        private void OnCompendiumExpanded(string compendiumName)
        {
            this.Logger.LogInformation("SelectedCompendium expanded: {CompendiumName}", compendiumName);

            // Update user selections
            this.SelectedSourceDataSetGrouping = null;
            this.SelectedSourceDataSet = null;
            this.SelectedIndex = 0;
        }

        /// <summary>
        /// Logs a message when the label assignment is changed and triggers a state change.
        /// </summary>
        private void OnLabelAssignmentChanged(string labelName)
        {
            this.Logger.LogInformation("Label assignment changed: {LabelName}", labelName);
            this.StateHasChanged();
        }

        /// <summary>
        /// Handles the label collapse event from the navigator component.
        /// </summary>
        /// <param name="labelName">The name of the label that was selected.</param>
        private void OnLabelCollapsed(string labelName)
        {
            this.Logger.LogInformation("Label collapsed: {LabelName}", labelName);

            // Update user selections
            this.SelectedSourceDataSetGrouping
                = this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(labelName), null);
            this.SelectedSourceDataSet = null;
            this.SelectedIndex = 1;
        }

        /// <summary>
        /// Handles the label expansion event from the navigator component.
        /// </summary>
        /// <param name="labelName">The name of the label that was selected.</param>
        private void OnLabelExpanded(string labelName)
        {
            this.Logger.LogInformation("Label expanded: {LabelName}", labelName);

            // Update user selections
            this.SelectedSourceDataSetGrouping
                = this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(labelName), null);
            this.SelectedSourceDataSet = null;
            this.SelectedIndex = 1;
        }

        /// <summary>
        /// Handles the SelectedSource clicked event by logging the selected SelectedSource name.
        /// </summary>
        /// <param name="values">The tuple containing the selected data set name and the lable it's associated with.</param>
        private async Task OnSourceDataSetSelected((string sourceDataSetName, string labelName) values)
        {
            this.Logger.LogInformation("Selected SelectedSource: {SourceDataSetName}", values.sourceDataSetName);

            // Update user selections
            this.SelectedSourceDataSetGrouping
                = this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(values.labelName), null);
            this.SelectedSourceDataSet
                = this.SelectedCompendium.SourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(values.sourceDataSetName), null);
            this.SelectedIndex = 3;
        }

    }
}