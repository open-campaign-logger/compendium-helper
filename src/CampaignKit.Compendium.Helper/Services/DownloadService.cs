﻿// <copyright file="DownloadService.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Services
{
    using CampaignKit.Compendium.Helper.Data;
    using CampaignKit.Compendium.Helper.Shared;

    using Microsoft.JSInterop;

    /// <summary>
    /// DownloadService class provides methods for downloading data to the client.
    /// </summary>
    public class DownloadService
    {
        /// <summary>
        /// Gets or sets the ILogger into the Logger property.
        /// </summary>
        private readonly ILogger<MainLayout> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadService"/> class.
        /// </summary>
        /// <param name="logger">A logger for log messages.</param>
        public DownloadService(ILogger<MainLayout> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Downloads a text string as a file using the JavaScript runtime.
        /// </summary>
        /// <param name="jsRuntime">The JavaScript runtime.</param>
        /// <param name="fileText">The text string to download.</param>
        /// <param name="fileName">The name of the file to save the JSON as.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DownloadTextFile(IJSRuntime jsRuntime, string fileText, string fileName)
        {
            this.logger.LogInformation("DownloadTextFile method called with fileName: {FileName}", RegexHelper.RemoveUnwantedCharactersFromLogMessage(fileName));
            var blob = $"data:text/fileText;charset=utf-8,{Uri.EscapeDataString(fileText)}";
            var script = $@"(function() {{
                        var link = document.createElement('a');
                        link.href = '{blob}';
                        link.download = '{fileName}';
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    }})();";
            await jsRuntime.InvokeVoidAsync("eval", script);
        }
    }
}
