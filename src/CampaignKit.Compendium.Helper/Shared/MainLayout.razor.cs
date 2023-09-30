// <copyright file="MainLayout.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Shared{    using CampaignKit.Compendium.Helper.Configuration;    using Microsoft.AspNetCore.Components;    using Microsoft.JSInterop;    using Radzen;

    /// <summary>
    /// Represents the main layout of the application.
    /// </summary>
    public partial class MainLayout    {
        /// <summary>
        /// Gets or sets the TooltipService property is used for injecting the TooltipService dependency.
        /// </summary>
        [Inject]        protected TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets or sets the ContextMenuService property is injected with the ContextMenuService dependency.
        /// </summary>
        [Inject]        private ContextMenuService ContextMenuService { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency into the property DialogService.
        /// </summary>
        [Inject]        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the IJSRuntime dependency into the property JsRuntime.
        /// </summary>
        [Inject]        private IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Gets or sets the ILogger into the Logger property.
        /// </summary>
        [Inject]        private ILogger<MainLayout> Logger { get; set; }

        /// <summary>
        /// Gets or sets the NavigationManager dependency into the property NavigationManager.
        /// </summary>
        [Inject]        private NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the NotificationService dependency into the property NotificationService.
        /// </summary>
        [Inject]        private NotificationService NotificationService { get; set; }

        /// <summary>
        /// Gets or sets the selected compendium.
        /// </summary>
        /// <value>The selected compendium.</value>
        private ICompendium SelectedCompendium { get; set; }

        /// <summary>
        /// Method called when the upload of a compendium is complete.
        /// </summary>
        /// <param name="compendium">The uploaded compendium.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnUploadComplete(ICompendium compendium)        {            this.SelectedCompendium = compendium;            this.DialogService.Close();        }

        /// <summary>
        /// Handles the event when a new compendium selection is made by the user.
        /// </summary>
        /// <param name="selection">The selection made by the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task OnNewCompendiumSelection(bool selection)        {            this.Logger.LogInformation("User selected {Selection}.", selection);            if (selection)            {                this.CreateDefaultCompendium();            }            this.DialogService.Close();        }

        private void CreateDefaultCompendium()
        {
            this.SelectedCompendium = new PublicCompendium();
        }

        /// <summary>
        /// Shows an "Upload File" dialog asynchronously and passes the OnUploadComplete callback method as a parameter.
        /// </summary>
        private async void ShowUploadDialog()        {
            await this.DialogService.OpenAsync<UploadDialog>(                "Upload Compendium",                new Dictionary<string, object> {                    { "Prompt", "Select an existing Compendium Configuration." },                    { "OnUploadComplete", EventCallback.Factory.Create<ICompendium>(this, this.OnUploadComplete) },                });        }

        /// <summary>
        /// Shows a new dialog asynchronously and waits for the user's selection. The dialog is opened using the DialogService with the specified title and prompt. The OnSelection event is subscribed to the OnNewCompendiumSelection method. The result of the dialog is displayed as an info notification.
        /// </summary>
        private async void ShowNewDialog()        {            if (this.SelectedCompendium == null)
            {
                this.CreateDefaultCompendium();
            }            else
            {
                await this.DialogService.OpenAsync<ConfirmationDialog>(
                    "New Compendium",
                    new Dictionary<string, object> {                    { "Prompt", "Replace the current Compendium Configuration?" },                    { "OnSelection", EventCallback.Factory.Create<bool>(this, this.OnNewCompendiumSelection) },
                    });
            }        }    }}