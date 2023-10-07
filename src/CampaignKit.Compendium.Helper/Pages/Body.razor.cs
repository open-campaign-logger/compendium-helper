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
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

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
        /// Gets or sets the BrowserService dependency.
        /// </summary>
        [Inject]
        private BrowserService BrowserService { get; set; }

        /// <summary>
        /// Gets or sets the JsRuntime dependency.
        /// </summary>
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

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
        private LabelGroup SelectedLabelGroup { get; set; }

        /// <summary>
        /// Method called after the component has been rendered.
        /// </summary>
        /// <param name="firstRender">Indicates if this is the first time the component is being rendered.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.UpdateTitle();
        }

        /// <summary>
        /// This method is called when the component's parameters are set. It first calls the base implementation of the method. Then, it sets the SelectedSourceDataSet and SelectedLabelGroup properties to null.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            this.SelectedSourceDataSet = null;
            this.SelectedLabelGroup = null;
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
            this.SelectedLabelGroup = null;
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
            this.SelectedLabelGroup = null;
            this.SelectedSourceDataSet = null;
            this.SelectedIndex = 0;
        }

        /// <summary>
        /// Event handler for when the title of the selected compendium changes.
        /// Logs the new title using the logger and triggers a state change.
        /// </summary>
        /// <param name="title">The new title of the selected compendium.</param>
        private async void OnCompendiumTitleChanged(string title)
        {
            this.Logger.LogInformation("SelectedCompendium title changed: {Title}", title);
            await this.UpdateTitle();
            this.StateHasChanged();
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
            this.SelectedLabelGroup
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
            this.SelectedLabelGroup
                = this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(labelName), null);
            this.SelectedSourceDataSet = null;
            this.SelectedIndex = 1;
        }

        /// <summary>
        /// Handles the LabelGroup clicked event by logging the selected LabelGroup name.
        /// </summary>
        /// <param name="values">The tuple containing the selected data set name and the lable it's associated with.</param>
        private async Task OnSourceDataSetSelected((string sourceDataSetName, string labelName) values)
        {
            this.Logger.LogInformation("Selected LabelGroup: {SourceDataSetName}", values.sourceDataSetName);

            // Update user selections
            this.SelectedLabelGroup
                = this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(values.labelName), null);
            this.SelectedSourceDataSet
                = this.SelectedCompendium.SourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(values.sourceDataSetName), null);
            this.SelectedIndex = 2;
        }

        /// <summary>
        /// Event handler for when the title of the selected SourceDataSet is changed.
        /// </summary>
        /// <param name="title">The new title of the SourceDataSet.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task OnSourceDataSetTitleChanged(string title)
        {
            this.Logger.LogInformation("Selected SourceDataSet title changed: {Title}", title);
            this.StateHasChanged();
        }

        /// <summary>
        /// Sets the page title of the browser using the selected compendium's title.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task UpdateTitle()
        {
            var title = string.IsNullOrEmpty(this.SelectedCompendium?.Title) ? "Compendium Helper" : this.SelectedCompendium.Title;
            await this.BrowserService.SetTitle(this.JsRuntime, title);
        }
    }
}