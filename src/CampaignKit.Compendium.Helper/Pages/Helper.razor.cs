// <copyright file="Helper.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using System;

    using CampaignKit.Compendium.Core.Configuration;
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Code behind class for Helper.razor.
    /// </summary>
    public partial class Helper
    {
        /// <summary>
        /// Gets or sets the EventCallback for requesting a refresh of the Compendium NavTree.
        /// </summary>
        public EventCallback CompendiumNavTreeRefreshRequested { get; set; }

        /// <summary>
        /// Gets or sets the CompendiumService.
        /// </summary>
        [Inject]
        protected CompendiumService CompendiumService { get; set; }

        /// <summary>
        /// Gets or sets the ContextMenuService.
        /// </summary>
        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        /// <summary>
        /// Gets or sets the DialogService.
        /// </summary>
        [Inject]
        protected DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the DownloadService.
        /// </summary>
        [Inject]
        protected DownloadService DownloadService { get; set; }

        /// <summary>
        /// Gets or sets a dictionary to store events and their associated timestamps.
        /// </summary>
        protected Dictionary<DateTime, string> Events { get; set; } = new Dictionary<DateTime, string>();

        /// <summary>
        /// Gets or sets the name of the uploaded file.
        /// </summary>
        protected string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source HTML for the current data set.
        /// </summary>
        protected string Html { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the HtmlService.
        /// </summary>
        [Inject]
        protected HtmlService HtmlService { get; set; }

        /// <summary>
        /// Gets or sets the JSRuntime for JS interop.
        /// </summary>
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        protected ILogger<Helper> Logger { get; set; }

        /// <summary>
        /// Gets or sets the Markdown conversion of the extracted HTML.
        /// </summary>
        protected string Markdown { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MarkdownService.
        /// </summary>
        [Inject]
        protected MarkdownService MarkdownService { get; set; }

        /// <summary>
        /// Gets or sets the NavigationManager.
        /// </summary>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the NotificationService.
        /// </summary>
        [Inject]
        protected NotificationService NotificationService { get; set; }

        /// <summary>
        /// Gets or sets a list of compendiums to store uploaded data.
        /// </summary>
        protected List<PublicCompendium> PublicCompendiums { get; set; } = new List<PublicCompendium>();

        /// <summary>
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        protected TooltipService TooltipService { get; set; }

        /// <summary>
        /// Logs an event with a given name and value.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="value">The value of the event.</param>
        protected void Log(string eventName, string value)
        {
            this.Events.Add(DateTime.Now, $"{eventName}: {value}");
        }

        /// <summary>
        /// Logs a change event with the item text from the TreeEventArgs.
        /// </summary>
        /// <param name="args">The TreeEventArgs containing the item text.</param>
        protected void LogChange(TreeEventArgs args)
        {
            this.Log("Change", $"Item Text: {args.Text}");
        }

        /// <summary>
        /// Logs the expand event of a tree item.
        /// </summary>
        /// <param name="args">The tree expand event arguments.</param>
        protected void LogExpand(TreeExpandEventArgs args)
        {
            if (args.Text is string text)
            {
                this.Log("Expand", $"Item Text: {text}");
            }
        }

        /// <summary>
        /// Uploads a compendium and creates a tree from it.
        /// </summary>
        /// <param name="args">The upload event arguments.</param>
        /// <returns>
        /// A tree created from the uploaded compendium.
        /// </returns>
        protected async Task UploadComplete(UploadCompleteEventArgs args)
        {
            this.Logger.LogInformation("Upload complete and converted to string of length {Length}.", args.RawResponse.Length);
            var json = args.RawResponse;
            this.PublicCompendiums = this.CompendiumService.LoadCompendiums(json);
        }
    }
}