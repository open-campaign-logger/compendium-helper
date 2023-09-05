// <copyright file="UploadController.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Default controller for handling file uploads.
    /// </summary>
    public partial class UploadController : Controller
   {
       /// <summary>
       /// Returns a file from the provided IFormFile.
       /// </summary>
       /// <param name="file">The file to be returned.</param>
       /// <returns>The file as an application/octet-stream.</returns>
       [HttpPost("upload/single")]
       public async Task<IActionResult> Single(IFormFile file)
       {
           try
           {
               if (file == null || file.Length == 0)
               {
                   return this.BadRequest("No file uploaded.");
               }

               using var reader = new StreamReader(file.OpenReadStream());
               var content = await reader.ReadToEndAsync();

               return this.Ok(content);
           }
           catch (Exception ex)
           {
               return this.StatusCode(500, ex.Message);
           }
       }

   }
}
