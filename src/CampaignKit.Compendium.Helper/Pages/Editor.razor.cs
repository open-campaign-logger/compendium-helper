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
    using Core.Configuration;
    using Services;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    /// <summary>
    /// Partial class for the Editor component.
    /// </summary>
    public partial class Editor
    {
        /// <summary>
        /// Gets or sets a value indicating whether the source data has customized content.
        /// </summary>
        public bool HasCustomizedContent { get; set; } = false;

        /// <summary>
        /// Gets or sets the selected source data set.
        /// </summary>
        [Parameter]
        public SourceDataSet Source { get; set; }

        /// <summary>
        /// Gets or sets the HtmlService.
        /// </summary>
        [Inject]
        private HtmlService HtmlService { get; set; }

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
        /// Gets or sets the Markdown service.
        /// </summary>
        [Inject]
        private MarkdownService MarkdownService { get; set; }

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
            ObjectReference?.Dispose();
        }

        /// <summary>
        /// Invoked when the content of the Markdown editor is changed.
        /// </summary>
        /// <param name="content">The new content of the Markdown editor.</param>
        [JSInvokable]
        public void OnContentChanged(string content)
        {
            Logger.LogInformation("OnChange with value parameter value: {Value}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(content));

            // Update the markdown property of the source data set
            Source.Markdown = content;

            // Convert the markdown to HTML
            var html = HtmlService.ConvertMarkdownToHtml(content);

            // Check if there is no existing substitution with XPath "//body" in the Source.Substitutions list
            if (Source.Substitutions.All(s => s.XPath != "//body"))
            {
                // If there is no existing substitution, create a new list with a single Substitution object
                Source.Substitutions = new List<Substitution>
                {
                    new()
                    {
                        XPath = "//body",
                        HTML = html,
                    },
                };
            }
            else
            {
                // If there is an existing substitution with XPath "//body", update its HTML property with the new value
                Source.Substitutions.First(s => s.XPath == "//body").HTML = html;
            }
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
            Logger.LogInformation("OnAfterRenderAsync with firstRender parameter value: {FirstRender}", firstRender);

            try
            {
                ObjectReference = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("window.simpleMDEInterop.setMarkdown", Source.Markdown, ObjectReference);
            }
            catch (JSException jsEx)
            {
                Logger.LogError(jsEx, "Unable to set markdown content in editor.");
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
            Logger.LogInformation("OnInitializedAsync");
        }

        /// <summary>
        /// Loads the source data set and logs the markdown.
        /// </summary>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        protected async override Task OnParametersSetAsync()
        {
            Logger.LogInformation("OnParametersSetAsync");
            await base.OnParametersSetAsync();
            try
            {
                // Download the web page source data
                if (Source != null)
                {
                    // Load the source data set
                    await SourceDataSetService.LoadSourceDataSetAsync(Source);

                    // Log the markdown
                    Logger.LogInformation("Source data loaded and converted to markdown: {Markdown}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(Source.Markdown));
                }
            }
            catch (JSException jsEx)
            {
                Logger.LogError(jsEx, "Unable to get HTML content from editor.");
            }
        }

        /// <summary>
        /// Enables or disables the editor based on the presence of a substitution element in the source.
        /// </summary>
        /// <returns>
        /// Nothing.
        /// </returns>
        protected async Task UpdateEditorMode()
        {
            if (Source.Substitutions.Any(s => s.XPath == "//body"))
            {
                HasCustomizedContent = true;
                await JSRuntime.InvokeVoidAsync("window.simpleMDEInterop.enableEditor()");
            }
            else
            {
                HasCustomizedContent = false;
                await JSRuntime.InvokeVoidAsync("window.simpleMDEInterop.disableEditor()");
            }
        }
    }
}
