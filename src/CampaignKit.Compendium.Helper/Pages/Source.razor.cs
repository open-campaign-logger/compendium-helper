// <copyright file="Source.razor.cs" company="Jochen Linnemann - IT-Service">
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

    using Radzen;

    /// <summary>
    /// Code behind for the SelectedSource component.
    /// </summary>
    public partial class Source
    {
        /// <summary>
        /// Gets or sets the SelectedLabelGroup property.
        /// </summary>
        /// <returns>The SelectedLabelGroup object.</returns>
        [Parameter]
        public SourceDataSet SelectedSource { get; set; } = new ();

        /// <summary>
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        public TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for source selection.
        /// </summary>
        [Parameter]
        public EventCallback<SourceDataSet> SelectedSourceChanged { get; set; }

        /// <summary>
        /// Method that is called when the selected source is changed.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnSelectedSourceChanged()
        {
            // Invoke callback
            await this.SelectedSourceChanged.InvokeAsync(this.SelectedSource);
        }

        /// <summary>
        /// Opens a tooltip with the specified content for the given element.
        /// </summary>
        /// <param name="elementReference">The element to open the tooltip for.</param>
        /// <param name="tooltip">The tooltip content.</param>
        /// <param name="options">Optional options for the tooltip.</param>
        private void ShowTooltip(ElementReference elementReference, string tooltip, TooltipOptions options = null)
        {
            this.TooltipService.Open(elementReference, tooltip, options);
        }
    }
}
