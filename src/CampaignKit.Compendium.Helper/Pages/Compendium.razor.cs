// <copyright file="Compendium.razor.cs" company="Jochen Linnemann - IT-Service">
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

    using Microsoft.AspNetCore.Components;

    using Radzen;

    /// <summary>
    /// Partial class for the SelectedCompendium component.
    /// </summary>
    public partial class Compendium
    {
        /// <summary>
        /// Gets or sets the SelectedCompendium object.
        /// </summary>
        /// <returns>The SelectedCompendium object.</returns>
        [Parameter]
        public ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Gets or sets the EventCallback for the compendium change events.
        /// </summary>
        [Parameter]
        public EventCallback<ICompendium> SelectedCompendiumChanged { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Asynchronously invokes the SelectedCompendiumChanged event with the current SelectedCompendium value.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous operation.
        /// </returns>
        private async Task OnSelectedCompendiumChanged()
        {
            await this.SelectedCompendiumChanged.InvokeAsync(this.SelectedCompendium);
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
