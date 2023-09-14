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
    using CampaignKit.Compendium.Core.Configuration;
    using CampaignKit.Compendium.Helper.Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

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
        /// Gets or sets the DownloadService.
        /// </summary>
        [Inject]
        private DownloadService DownloadService { get; set; }

        /// <summary>
        /// Gets or sets the HTMLService.
        /// </summary>
        [Inject]
        private HtmlService HtmlService { get; set; }

        /// <summary>
        /// Gets or sets the downloaded HTML.
        /// </summary>
        private string HTML { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JSRuntime for JS interop.
        /// </summary>
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the Logger.
        /// </summary>
        [Inject]
        private ILogger<Editor> Logger { get; set; }

        /// <summary>
        /// Gets or sets the Markdown conversion of the extracted HTML.
        /// </summary>
        private string Markdown { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MarkdownService.
        /// </summary>
        [Inject]
        private MarkdownService MarkdownService { get; set; }

        /// <summary>
        /// Gets or sets property to store a reference to an Editor object.
        /// </summary>
        private DotNetObjectReference<Editor> ObjectReference { get; set; }

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
                await this.JSRuntime.InvokeVoidAsync("window.simpleMDEInterop.setMarkdown", this.Markdown, this.ObjectReference);
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
        protected async override Task OnInitializedAsync()
        {
            this.Logger.LogInformation("OnInitializedAsync");

            try
            {
                // Set the markdown to "Loading..."
                this.Markdown = "# Loading...";

                // Download the web page asynchronously
                this.HTML = await this.DownloadService.GetWebPageAync(this.Source.SourceDataSetURI);

                // Log the HTML
                this.Logger.LogInformation("HTML: {HTML}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(this.HTML));

                // Set the markdown to "Converting..."
                this.Markdown = "# Converting...";

                // Convert the HTML to markdown
                this.Markdown = this.MarkdownService.ConvertHtmlToMarkdown(this.HTML);

                // Log the markdown
                this.Logger.LogInformation("Markdown: {Markdown}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(this.Markdown));
            }
            catch (JSException jsEx)
            {
                // Log the error
                this.Markdown = "# Unable to load content...";

                this.Logger.LogError(jsEx, "Unable to get HTML content from editor.");
            }
        }
    }
}
