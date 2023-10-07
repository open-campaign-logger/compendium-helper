// <copyright file="FetchException.cs" company="Jochen Linnemann - IT-Service">
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
    /// Represents an exception that occurs when there is an error fetching data.
    /// </summary>
    public class FetchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <returns>
        /// FetchException object.
        /// </returns>
        public FetchException(string message)
                   : base(message)
        {
        }

        ///<summary>
        /// Initializes a new instance of the <see cref="FetchException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        /// <returns>
        /// FetchException object.
        /// </returns>
        public FetchException(string message, Exception innerException)
                   : base(message, innerException)
        {
        }
    }
}
