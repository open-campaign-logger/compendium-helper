// <copyright file="ConfirmationDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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
// </copyright>namespace CampaignKit.Compendium.Helper.Shared{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Represents a partial class for a confirmation dialog.
    /// </summary>
    public partial class ConfirmationDialog    {
        /// <summary>
        /// Gets or sets the event callback for the OnSelection event.
        /// The event callback takes a boolean parameter and returns void.
        /// </summary>
        [Parameter]        public EventCallback<bool> OnSelection { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the parameter.
        /// </summary>
        /// <value>The prompt.</value>
        [Parameter]        public string Prompt { get; set; }

        /// <summary>
        /// Handles the event when the "Yes" option is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnYes()        {            await this.OnSelection.InvokeAsync(true);        }

        /// <summary>
        /// Handles the event when the "No" option is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnNo()        {            await this.OnSelection.InvokeAsync(false);        }    }}