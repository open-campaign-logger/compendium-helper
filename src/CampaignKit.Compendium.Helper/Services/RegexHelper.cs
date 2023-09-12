using System.Text.RegularExpressions;
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

    /// <summary>
    /// Utility class for generating Regex objects.
    /// </summary>
    internal static partial class RegexHelper
    {

        /// <summary>
        /// Generates a Regex to select wanted characters.
        /// </summary>
        /// <returns>
        /// A Regex to select characters that are not alphanumeric, curly brackets, or square brackets.
        /// </returns>
        [GeneratedRegex("[^a-zA-Z0-9{\\[\\]}]")]
        public static partial Regex SelectWantedCharacters();

        /// <summary>
        /// Removes unwanted characters from a log message, up to a maximum length.
        /// </summary>
        /// <param name="message">The log message to be processed.</param>
        /// <param name="maxLength">The maximum length of the log message.</param>
        /// <returns>The log message with unwanted characters removed.</returns>
        public static string RemoveUnwantedCharactersFromLogMessage(string message, int maxLength = 100)
        {
            return SelectWantedCharacters().Replace(message[..Math.Min(maxLength, message.Length)], string.Empty);
        }
    }
}