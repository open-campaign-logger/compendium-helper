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

namespace CampaignKit.Compendium.Helper.Pages{    using CampaignKit.Compendium.Helper.Configuration;    using CampaignKit.Compendium.Helper.Data;    using CampaignKit.Compendium.Helper.Services;    using Microsoft.AspNetCore.Components;    using Microsoft.JSInterop;

    /// <summary>
    /// Represents the body of a class or struct.
    /// </summary>
    public partial class Body    {
        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        /// <value>The selected compendium.</value>
        [Parameter]        public ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when the selected compendium is changed.
        /// </summary>
        /// <value>The event callback for when the selected compendium is changed.</value>
        [Parameter]        public EventCallback<ICompendium> SelectedCompendiumChanged { get; set; }

        /// <summary>
        /// Gets or sets the selected label group.
        /// </summary>
        /// <value>The selected label group.</value>
        [Parameter]        public LabelGroup SelectedLabelGroup { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when the selected label group changes.
        /// </summary>
        /// <value>The event callback for the selected label group change.</value>
        [Parameter]        public EventCallback<LabelGroup> SelectedLabelGroupChanged { get; set; }

        /// <summary>
        /// Gets or sets the selected source data set.
        /// </summary>
        /// <value>The selected source data set.</value>
        [Parameter]        public SourceDataSet SelectedSourceDataSet { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when the selected source data set is changed.
        /// </summary>
        /// <value>The event callback for the selected source data set.</value>
        [Parameter]        public EventCallback<SourceDataSet> SelectedSourceDataSetChanged { get; set; }

        /// <summary>
        /// Gets or sets the BrowserService dependency.
        /// </summary>
        [Inject]        private BrowserService BrowserService { get; set; }

        /// <summary>
        /// Gets or sets the IJSRuntime dependency.
        /// </summary>
        [Inject]        private IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Gets or sets the ILogger dependency.
        /// </summary>
        [Inject]        private ILogger<Body> Logger { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        private int SelectedIndex { get; set; }

        /// <summary>
        /// Method called after the component has been rendered.
        /// </summary>
        /// <param name="firstRender">Indicates if this is the first time the component is being rendered.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)        {            await this.UpdateTitle();        }

        /// <summary>
        /// Event handler for when a compendium is collapsed.
        /// </summary>
        /// <param name="compendiumName">The name of the collapsed compendium.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task OnCompendiumCollapsed(string compendiumName)        {            this.Logger.LogInformation("SelectedCompendium collapsed: {CompendiumName}", compendiumName);            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            await this.SelectedLabelGroupChanged.InvokeAsync(null);            this.SelectedIndex = 0;        }

        /// <summary>
        /// Event handler for when a compendium is expanded.
        /// </summary>
        /// <param name="compendiumName">The name of the expanded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnCompendiumExpanded(string compendiumName)        {            this.Logger.LogInformation("SelectedCompendium expanded: {CompendiumName}", compendiumName);            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            await this.SelectedLabelGroupChanged.InvokeAsync(null);            this.SelectedIndex = 0;        }

        /// <summary>
        /// Event handler for when the Compendium title is changed. 
        /// Logs the new title using the Logger and updates the title asynchronously.
        /// Calls StateHasChanged to trigger a UI update.
        /// </summary>
        private async void OnCompendiumTitleChanged(string title)        {            this.Logger.LogInformation("SelectedCompendium title changed: {Title}", title);            await this.UpdateTitle();            this.StateHasChanged();        }

        /// <summary>
        /// Event handler for when the label assignment changes.
        /// Logs the label name and updates the state.
        /// </summary>
        /// <param name="labelName">The name of the label that was assigned.</param>
        private void OnLabelAssignmentChanged(string labelName)        {            this.Logger.LogInformation("Label assignment changed: {LabelName}", labelName);            this.StateHasChanged();        }

        /// <summary>
        /// Event handler for when a label is collapsed.
        /// </summary>
        /// <param name="labelName">The name of the collapsed label.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnLabelCollapsed(string labelName)        {            this.Logger.LogInformation("Label collapsed: {LabelName}", labelName);            await this.SelectedLabelGroupChanged.InvokeAsync(this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(labelName), null));            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            this.SelectedIndex = 1;        }

        /// <summary>
        /// Event handler for when a label is expanded.
        /// </summary>
        /// <param name="labelName">The name of the label that was expanded.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnLabelExpanded(string labelName)        {            this.Logger.LogInformation("Label expanded: {LabelName}", labelName);            await this.SelectedLabelGroupChanged.InvokeAsync(this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(labelName), null));            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            this.SelectedIndex = 1;        }

        /// <summary>
        /// Event handler for when a source data set is selected.
        /// </summary>
        /// <param name="values">A tuple containing the source data set name and label name.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSourceDataSetSelected((string sourceDataSetName, string labelName) values)        {            this.Logger.LogInformation("Selected LabelGroup: {SourceDataSetName}", values.sourceDataSetName);            await this.SelectedLabelGroupChanged.InvokeAsync(this.SelectedCompendium.SourceDataSetGroupings.FirstOrDefault(sdsg => sdsg.LabelName.Equals(values.labelName), null));            await this.SelectedSourceDataSetChanged.InvokeAsync(this.SelectedCompendium.SourceDataSets.FirstOrDefault(sds => sds.SourceDataSetName.Equals(values.sourceDataSetName), null));            this.SelectedIndex = 2;        }

        /// <summary>
        /// Event handler for when the title of the selected SourceDataSet changes.
        /// </summary>
        /// <param name="title">The new title of the SourceDataSet.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task OnSourceDataSetTitleChanged(string title)        {            this.Logger.LogInformation("Selected SourceDataSet title changed: {Title}", title);            this.StateHasChanged();        }

        /// <summary>
        /// Updates the title of the browser window with the title of the selected compendium, or sets it to "Compendium Helper" if no compendium is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task UpdateTitle()        {            var title = string.IsNullOrEmpty(this.SelectedCompendium?.Title) ? "Compendium Helper" : this.SelectedCompendium.Title;            await this.BrowserService.SetTitle(this.JsRuntime, title);        }    }}