﻿// <copyright file="Compendium.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

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
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        private TooltipService TooltipService { get; set; }

        /// <summary>
        /// Overrides the OnAfterRenderAsync method and calls the SetBrowserPageTitle method asynchronously.
        /// </summary>
        /// <param name="firstRender">A boolean value indicating whether this is the first render of the component.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.SetBrowserPageTitle();
        }

        /// <summary>
        /// This method is called when the component is being initialized asynchronously.
        /// It first calls the base implementation of OnInitializedAsync() to ensure that the component is properly initialized.
        /// Then it calls the SetBrowserPageTitle() method asynchronously to set the title of the browser page.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Event handler for when the compendium title is changed.
        /// </summary>
        /// <param name="title">The new title of the compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnCompendiumTitleChanged(string title)
        {
            await this.SetBrowserPageTitle();
        }

        /// <summary>
        /// Sets the page title of the browser using the selected compendium's title.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task SetBrowserPageTitle()
        {
            var title = this.SelectedCompendium?.Title ?? string.Empty;
            await this.BrowserService.SetTitle(this.JsRuntime, title);
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
