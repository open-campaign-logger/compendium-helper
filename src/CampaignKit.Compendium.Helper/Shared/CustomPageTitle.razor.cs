// <copyright file="CustomPageTitle.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Shared{    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    /// <summary>
    /// Represents a custom page title for a web page.
    /// </summary>
    public partial class CustomPageTitle    {
        /// <summary>
        /// Gets or sets the title parameter.
        /// </summary>
        [Parameter]        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the JSRuntime dependency.
        /// </summary>
        [Inject]        private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the ILogger dependency.
        /// </summary>
        [Inject]        private ILogger<CustomPageTitle> Logger { get; set; }

        /// <summary>
        /// Overrides the OnAfterRenderAsync method and calls the base implementation. Then, it calls the SetTitle method asynchronously.
        /// </summary>
        /// <param name="firstRender">A boolean value indicating whether this is the first render of the component.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await this.SetTitle();
        }

        /// <summary>
        /// Sets the title of the page using JavaScript runtime.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task SetTitle()
        {
            if (string.IsNullOrEmpty(this.Title))            {                return;            }            try            {                this.Logger.LogDebug("Setting title to: {}", this.Title);                await this.JSRuntime.InvokeVoidAsync("setTitle", this.Title);            }            catch (Exception ex)            {                this.Logger.LogError(ex, "Error setting page title");            }
        }
    }
}