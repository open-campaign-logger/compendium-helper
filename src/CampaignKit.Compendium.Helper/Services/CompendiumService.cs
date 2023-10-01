// <copyright file="CompendiumService.cs" company="Jochen Linnemann - IT-Service">
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
    using CampaignKit.Compendium.Helper.Configuration;
    using CampaignKit.Compendium.Helper.Data;

    using Newtonsoft.Json;

    /// <summary>
    /// CompendiumService provides methods for loading compendiums.
    /// </summary>
    public partial class CompendiumService
    {
        private readonly ILogger<CompendiumService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompendiumService"/> class.
        /// </summary>
        /// <param name="logger">Logger object for logging.</param>
        /// <returns>
        /// DownloadService object.
        /// </returns>
        public CompendiumService(ILogger<CompendiumService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Loads a PublicCompendium object from a JSON string.
        /// If multiple compendiums are found only the first one is returned.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>A PublicCompendium object.</returns>
        public ICompendium LoadCompendium(string json)
        {
            // Validate parameters
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            // Log method entry.
            this.logger.LogInformation("LoadCompendium method called with JSON: {JSON}.", RegexHelper.RemoveUnwantedCharactersFromLogMessage(json));

            // Deserialize the JSON string into a Dictionary object using Newtonsoft.Json
            Dictionary<string, List<PublicCompendium>> dictionary;
            try
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<PublicCompendium>>>(json);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Unable to deserialize JSON into list of PublicCompendium objects.");
                throw;
            }

            // Validate that the dictionary contains a "compendium" key.
            if (dictionary == null || dictionary.Keys.Count != 1)
            {
                throw new Exception("Unable to deserialize JSON into list of PublicCompendium objects.");
            }

            // Get the List of PublicCompendium objects from the dictionary.
            List<PublicCompendium> compendiumList = dictionary.Values.FirstOrDefault(new List<PublicCompendium>())
                ?? throw new Exception("Unable to deserialize JSON into list of PublicCompendium objects.");

            // Return the PublicCompendium object.
            return compendiumList.FirstOrDefault(new PublicCompendium());
        }

        /// <summary>
        /// Saves the provided compendium object as a JSON string.
        /// </summary>
        /// <param name="compendium">The compendium object to be saved.</param>
        /// <returns>A JSON string representation of the compendium object.</returns>
        public string SaveCompendium(ICompendium compendium)
        {
            // Validate parameters
            if (compendium == null)
            {
                throw new ArgumentNullException(nameof(compendium));
            }

            Dictionary<string, List<PublicCompendium>> dictionary
                = new Dictionary<string, List<PublicCompendium>> { { "WebScraperPublicCompendiums", new List<PublicCompendium> { (PublicCompendium)compendium } } };

            // Serialize compendium into a JSON string using Newtonsoft.Json.
            string json = JsonConvert.SerializeObject(dictionary);
            return json;
        }
    }
}