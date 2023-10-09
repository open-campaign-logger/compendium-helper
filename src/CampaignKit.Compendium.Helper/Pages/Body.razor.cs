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
        /// Gets or sets the list of label groups.
        /// </summary>
        /// <value>The list of label groups.</value>
        [Parameter]
        public List<LabelGroup> LabelGroups { get; set; }

        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        /// <value>The selected compendium.</value>
        [Parameter]        public ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when the selected compendium changes.
        /// </summary>
        /// <value>The event callback for when the selected compendium changes.</value>
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
        [Parameter]        public SourceDataSet SelectedSource { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when the selected source is changed.
        /// </summary>
        /// <value>The event callback for the selected source change.</value>
        [Parameter]        public EventCallback<SourceDataSet> SelectedSourceChanged { get; set; }

        /// <summary>
        /// Gets or sets the list of temporary labels.
        /// </summary>
        /// <value>The temporary labels.</value>
        [Parameter]        public List<string> TemporaryLabels { get; set; }

        /// <summary>
        /// Gets or sets the ILogger dependency.
        /// </summary>
        [Inject]        private ILogger<Body> Logger { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        /// <value>The index of the selected item.</value>
        private int SelectedIndex { get; set; }

        /// <summary>
        /// Event handler for when the selected compendium is changed.
        /// </summary>
        /// <param name="compendium">The new selected compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSelectedCompendiumChanged(ICompendium compendium)        {            this.Logger.LogInformation("Selected compendium changed: {CompendiumName}", compendium.Title);            await this.SelectedCompendiumChanged.InvokeAsync(compendium);            this.SelectedIndex = 0;        }

        /// <summary>
        /// Event handler for when the selected label group is changed.
        /// </summary>
        /// <param name="labelGroup">The new selected label group.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSelectedLabelGroupChanged(LabelGroup labelGroup)        {            this.Logger.LogInformation("Selected label changed: {LabelName}", labelGroup.LabelName);            await this.SelectedLabelGroupChanged.InvokeAsync(labelGroup);            this.SelectedIndex = 1;        }

        /// <summary>
        /// Event handler for when the selected source is changed.
        /// </summary>
        /// <param name="values">A tuple containing the new source dataset and label group.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnSelectedSourceChanged((SourceDataSet sourceDataSet, LabelGroup labelGroup) values)        {            this.Logger.LogInformation("Selected source changed: {SourceDataSetName}", values.sourceDataSet.SourceDataSetName);            if (values.labelGroup != null)
            {                await this.SelectedLabelGroupChanged.InvokeAsync(values.labelGroup);
            }            await this.SelectedSourceChanged.InvokeAsync(values.sourceDataSet);            this.SelectedIndex = 2;        }    }}