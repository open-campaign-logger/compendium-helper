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
    using CampaignKit.Compendium.Helper.Dialogs;
    using CampaignKit.Compendium.Helper.Services;

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
        public SourceDataSet SelectedSource { get; set; }

        /// <summary>
        /// Gets or sets the event callback for when the selected source is changed.
        /// </summary>
        [Parameter]
        public EventCallback<SourceDataSet> SelectedSourceChanged { get; set; }

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
            this.Logger.LogInformation("OnContentChanged: {Value}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(content));
            this.SelectedSource.Markdown = content;
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
                await this.JsRuntime.InvokeVoidAsync("window.simpleMDEInterop.setMarkdown", this.SelectedSource.Markdown, this.ObjectReference);
            }
            catch (JSException jsEx)
            {
                this.Logger.LogError(jsEx, "Unable to set markdown content in editor.");
            }
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
                if (this.SelectedSource != null)
                {
                    await this.SourceDataSetService.LoadSourceDataSetAsync(this.SelectedSource, forceReload);
                }

                if (forceReload)
                {
                    await this.JsRuntime.InvokeVoidAsync("window.simpleMDEInterop.setMarkdown", this.SelectedSource.Markdown, this.ObjectReference);
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
