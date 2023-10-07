// <copyright file="Editor.razor.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Services;
    using CampaignKit.Compendium.Helper.Dialogs;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Partial class for the Editor component.
    /// </summary>
    public partial class Editor
    {
        /// <summary>
        /// Gets or sets the selected source data set.
        /// </summary>
        [Parameter]
        public SourceDataSet Source { get; set; }

        /// <summary>
        /// Gets or sets the DialogService dependency.
        /// </summary>
        [Inject]
        private DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the JSRuntime for JS interop.
        /// </summary>
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<Editor> Logger { get; set; }

        /// <summary>
        /// Gets or sets property to store a reference to an Editor object.
        /// </summary>
        private DotNetObjectReference<Editor> ObjectReference { get; set; }

        /// <summary>
        /// Gets or sets the SourceDataSetService.
        /// </summary>
        [Inject]
        private SourceDataSetService SourceDataSetService { get; set; }

        /// <summary>
        /// Disposes the object reference.
        /// </summary>
        public void Dispose()
        {
            this.ObjectReference?.Dispose();
        }

        /// <summary>
        /// Invoked when the content of the Markdown editor is changed.
        /// </summary>
        /// <param name="content">The new content of the Markdown editor.</param>
        [JSInvokable]
        public void OnContentChanged(string content)
        {
            this.Logger.LogInformation("OnChange with value parameter value: {Value}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(content));

            // Update the markdown property of the source data set
            this.Source.Markdown = content;
        }

        /// <summary>
        /// Method that is invoked when a reload event occurs.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        [JSInvokable]
        public async Task OnReload()
        {
            this.Logger.LogInformation("OnReload");
            await this.LoadContent(true);
        }

        /// <summary>
        /// Invokes the JavaScript function to set the markdown.
        /// </summary>
        /// <param name="firstRender">A boolean value indicating whether this is the first render.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            this.Logger.LogInformation("OnAfterRenderAsync with firstRender parameter value: {FirstRender}", firstRender);

            try
            {
                this.ObjectReference = DotNetObjectReference.Create(this);
                await this.JsRuntime.InvokeVoidAsync("window.simpleMDEInterop.setMarkdown", this.Source.Markdown, this.ObjectReference);
                await this.JsRuntime.InvokeVoidAsync("window.simpleMDEInterop.disableEditor"); // Start in preview mode.
            }
            catch (JSException jsEx)
            {
                this.Logger.LogError(jsEx, "Unable to set markdown content in editor.");
            }
        }

        /// <summary>
        /// Initializes the component, retrieves the HTML for ths source, and converts it to Markdown.
        /// </summary>
        /// <returns>
        /// The HTML and Markdown content from the editor.
        /// </returns>
        protected override async Task OnInitializedAsync()
        {
            this.Logger.LogInformation("OnInitializedAsync");
        }

        /// <summary>
        /// Loads the source data set and logs the markdown.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        protected override async Task OnParametersSetAsync()
        {
            this.Logger.LogInformation("OnParametersSetAsync");
            await base.OnParametersSetAsync();
            await this.LoadContent();
        }

        /// <summary>
        /// Loads the content by downloading the web page source data and converting it to markdown format.
        /// </summary>
        /// <param name="forceReload">A value indicating whether to force a reload of the source data set.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        private async Task LoadContent(bool forceReload = false)
        {
            try
            {
                // Download the web page source data
                if (this.Source != null)
                {
                    // Load the source data set
                    await this.SourceDataSetService.LoadSourceDataSetAsync(this.Source, forceReload);

                    // Log the markdown
                    this.Logger.LogInformation("Source data loaded and converted to markdown: {Markdown}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(this.Source.Markdown));
                }
            }
            catch (FetchException fe)
            {
                await this.DialogService.OpenAsync<MessageDialog>(
                    "Download Error",
                    new Dictionary<string, object>                    {                        { "Prompt", fe.Message },
                    });
            }
        }
    }
}
