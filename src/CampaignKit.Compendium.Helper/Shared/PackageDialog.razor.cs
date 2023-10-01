// <copyright file="PackageDialog.razor.cs" company="Jochen Linnemann - IT-Service">
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


namespace CampaignKit.Compendium.Helper.Shared
{
    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Code behind class for PackageDialog.
    /// </summary>
    public partial class PackageDialog
    {
        /// <summary>
        /// Gets or sets the event callback for when the upload is complete.
        /// The event callback takes an ICompendium parameter.
        /// </summary>
        [Parameter]
        public EventCallback<ICompendium> OnUploadComplete { get; set; }

        /// <summary>
        /// Gets or sets the package filename.
        /// </summary>
        [Parameter]
        public string PackageFileName { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the parameter.
        /// </summary>
        /// <value>The prompt.</value>
        [Parameter]
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the CompendiumService dependency into the CompendiumService property.
        /// </summary>
        [Inject]
        private CompendiumService CompendiumService { get; set; }

        /// <summary>
        /// Gets or sets the FileService dependency into the FileService property.
        /// </summary>
        [Inject]
        private FileService FileService { get; set; }

        /// <summary>
        /// Gets or sets injects an ILogger dependency into the Logger property.
        /// </summary>
        [Inject]
        private ILogger<PackageDialog> Logger { get; set; }

        /// <summary>
        /// Handles the event when the "No" option is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnNo()
        {
            this.Logger.LogInformation("User selected No.");
            await this.OnUploadComplete.InvokeAsync(null);
        }

        /// <summary>
        /// Handles the event when the "Yes" option is selected.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task OnYes()
        {
            this.Logger.LogInformation("User selected Yes.");
            var json = await this.FileService.ReadPackageFileAsync(this.PackageFileName);
            var compendium = this.CompendiumService.LoadCompendium(json);
            await this.OnUploadComplete.InvokeAsync(compendium);
        }
    }
}
