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
        /// Gets or sets the selected label.
        /// </summary>
        /// <value>The selected label.</value>
        [Parameter]        public LabelGroup SelectedLabelGroup { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when the selected label changes.
        /// </summary>
        /// <value>The event callback for the selected label changes.</value>
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
        /// <param name="compendium">The compendium that was collapsed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnCompendiumCollapsed(ICompendium compendium)        {            this.Logger.LogInformation("SelectedCompendium collapsed: {CompendiumName}", compendium.Title);            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            await this.SelectedLabelGroupChanged.InvokeAsync(null);            this.SelectedIndex = 0;        }

        /// <summary>
        /// Event handler for when a compendium is expanded.
        /// </summary>
        /// <param name="compendium">The expanded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnCompendiumExpanded(ICompendium compendium)        {            this.Logger.LogInformation("SelectedCompendium expanded: {CompendiumName}", compendium.Title);            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            await this.SelectedLabelGroupChanged.InvokeAsync(null);            this.SelectedIndex = 0;        }

        /// <summary>
        /// Event handler for when the Compendium title is changed.
        /// Logs the new title using the Logger and updates the title asynchronously.
        /// Calls StateHasChanged to trigger a UI update.
        /// </summary>
        private async void OnCompendiumTitleChanged(string title)        {            this.Logger.LogInformation("SelectedCompendium title changed: {Title}", title);            await this.UpdateTitle();        }

        /// <summary>
        /// Event handler for when the label assignment changes.
        /// Logs the label name and updates the state.
        /// </summary>
        /// <param name="labelName">The name of the label that was assigned.</param>
        private void OnLabelAssignmentChanged(string labelName)        {            this.Logger.LogInformation("Label assignment changed: {LabelName}", labelName);        }

        /// <summary>
        /// Event handler for when a label is collapsed.
        /// </summary>
        /// <param name="labelGroup">The label group that was collapsed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnLabelCollapsed(LabelGroup labelGroup)        {            this.Logger.LogInformation("Label collapsed: {LabelName}", labelGroup.LabelName);            await this.SelectedLabelGroupChanged.InvokeAsync(labelGroup);            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            this.SelectedIndex = 1;        }

        /// <summary>
        /// Event handler for when a label is expanded in a label group.
        /// </summary>
        /// <param name="labelGroup">The label group that was expanded.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnLabelExpanded(LabelGroup labelGroup)        {            this.Logger.LogInformation("Label expanded: {LabelName}", labelGroup.LabelName);            await this.SelectedLabelGroupChanged.InvokeAsync(labelGroup);            await this.SelectedSourceDataSetChanged.InvokeAsync(null);            this.SelectedIndex = 1;        }

        /// <summary>
        /// Event handler for when a source data set is selected.
        /// </summary>
        /// <param name="values">A tuple containing the source data set name and label name.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSourceDataSetSelected((SourceDataSet sourceDataSet, LabelGroup labelGroup) values)        {            this.Logger.LogInformation("Selected LabelGroup: {SourceDataSetName}", values.sourceDataSet.SourceDataSetName);            await this.SelectedLabelGroupChanged.InvokeAsync(values.labelGroup);            await this.SelectedSourceDataSetChanged.InvokeAsync(values.sourceDataSet);            this.SelectedIndex = 2;        }

        /// <summary>
        /// Event handler for when the title of the selected SourceDataSet changes.
        /// </summary>
        /// <param name="title">The new title of the SourceDataSet.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task OnSourceDataSetTitleChanged(string title)        {            this.Logger.LogInformation("Selected SourceDataSet title changed: {Title}", title);        }

        /// <summary>
        /// Updates the title of the browser window with the title of the selected compendium, or sets it to "Compendium Helper" if no compendium is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task UpdateTitle()        {            var title = string.IsNullOrEmpty(this.SelectedCompendium?.Title) ? "Compendium Helper" : this.SelectedCompendium.Title;            await this.BrowserService.SetTitle(this.JsRuntime, title);        }    }}