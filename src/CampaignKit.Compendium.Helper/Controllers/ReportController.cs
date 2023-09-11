// <copyright file="ReportController.cs" company="Jochen Linnemann - IT-Service">
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
    using System.Net;
    using System.Text;

    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller for SSRS report proxy.
    /// </summary>
    public partial class ReportController : Controller
    {
        /// <summary>
        /// Forwards the request to the specified URL.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="currentReqest">The current request.</param>
        /// <param name="url">The URL.</param>
        /// <returns>The response from the forwarded request.</returns>
        public async Task<HttpResponseMessage> ForwardRequest(HttpClient httpClient, HttpRequest currentReqest, string url)
        {
            var proxyRequestMessage = new HttpRequestMessage(new HttpMethod(currentReqest.Method), url);

            foreach (var header in currentReqest.Headers)
            {
                if (header.Key != "Host")
                {
                    proxyRequestMessage.Headers.TryAddWithoutValidation(header.Key, new string[] { header.Value });
                }
            }

            this.OnReportRequest(ref proxyRequestMessage);

            if (currentReqest.Method == "POST")
            {
                using var stream = new MemoryStream();
                await currentReqest.Body.CopyToAsync(stream);
                stream.Position = 0;

                string body = new StreamReader(stream).ReadToEnd();
                proxyRequestMessage.Content = new StringContent(body);

                if (body.IndexOf("AjaxScriptManager") != -1)
                {
                    proxyRequestMessage.Content.Headers.Remove("Content-Type");
                    proxyRequestMessage.Content.Headers.Add("Content-Type", new string[] { currentReqest.ContentType });
                }
            }

            return await httpClient.SendAsync(proxyRequestMessage);
        }

        /// <summary>
        /// Forwards an HTTP request to a given URL and returns the response.
        /// </summary>
        /// <param name="url">The URL to forward the request to.</param>
        /// <returns>The response from the forwarded request.</returns>
        [HttpGet("/__ssrsreport")]
        public async Task Get(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                using var httpClient = this.CreateHttpClient();
                var responseMessage = await this.ForwardRequest(httpClient, this.Request, url);

                CopyResponseHeaders(responseMessage, this.Response);

                await WriteResponse(this.Request, url, responseMessage, this.Response, false);
            }
        }

        /// <summary>
        /// Proxies a request to the SSRS server.
        /// </summary>
        /// <returns>
        /// The response from the SSRS server.
        /// </returns>
        [Route("/ssrsproxy/{*url}")]
        public async Task Proxy()
        {
            var urlToReplace = string.Format("{0}://{1}{2}/{3}/", this.Request.Scheme, this.Request.Host.Value, this.Request.PathBase, "ssrsproxy");
            var requestedUrl = this.Request.GetDisplayUrl().Replace(urlToReplace, string.Empty, StringComparison.InvariantCultureIgnoreCase);
            var reportServerIndex = requestedUrl.IndexOf("/ReportServer", StringComparison.InvariantCultureIgnoreCase);
            if (reportServerIndex == -1)
            {
                reportServerIndex = requestedUrl.IndexOf("/Reports", StringComparison.InvariantCultureIgnoreCase);
            }

            var reportUrlParts = requestedUrl[..reportServerIndex].Split('/');

            var url = string.Format("{0}://{1}:{2}{3}", reportUrlParts[0], reportUrlParts[1], reportUrlParts[2], requestedUrl[reportServerIndex..]);

            using var httpClient = this.CreateHttpClient();
            var responseMessage = await this.ForwardRequest(httpClient, this.Request, url);

            CopyResponseHeaders(responseMessage, this.Response);

            if (this.Request.Method == "POST")
            {
                await WriteResponse(this.Request, url, responseMessage, this.Response, true);
            }
            else
            {
                if (responseMessage.Content.Headers.ContentType != null && responseMessage.Content.Headers.ContentType.MediaType == "text/html")
                {
                    await WriteResponse(this.Request, url, responseMessage, this.Response, false);
                }
                else
                {
                    using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
                    await responseStream.CopyToAsync(this.Response.Body, 81920, this.HttpContext.RequestAborted);
                }
            }
        }

        /// <summary>
        /// Copies the response headers from an HttpResponseMessage to an HttpResponse.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="response">The HttpResponse.</param>
        private static void CopyResponseHeaders(HttpResponseMessage responseMessage, HttpResponse response)
        {
            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            response.Headers.Remove("transfer-encoding");
        }

        /// <summary>
        /// Writes the response to the HttpResponse object.
        /// </summary>
        /// <param name="currentReqest">The current request.</param>
        /// <param name="url">The URL.</param>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="response">The response.</param>
        /// <param name="isAjax">if set to <c>true</c> [is ajax].</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private static async Task WriteResponse(HttpRequest currentReqest, string url, HttpResponseMessage responseMessage, HttpResponse response, bool isAjax)
        {
            var result = await responseMessage.Content.ReadAsStringAsync();

            var reportServer = url.Contains("/ReportServer/", StringComparison.InvariantCultureIgnoreCase) ? "ReportServer" : "Reports";

            var reportUri = new Uri(url);
            var proxyUrl = $"{currentReqest.Scheme}://{currentReqest.Host.Value}{currentReqest.PathBase}/ssrsproxy/{reportUri.Scheme}/{reportUri.Host}/{reportUri.Port}";

            if (isAjax && result.IndexOf("|") != -1)
            {
                var builder = new StringBuilder();
                var index = 0;

                while (index < result.Length)
                {
                    int delimiterIndex = result.IndexOf("|", index);
                    if (delimiterIndex == -1)
                    {
                        break;
                    }

                    int length = int.Parse(result[index..delimiterIndex]);
                    if ((length % 1) != 0)
                    {
                        break;
                    }

                    index = delimiterIndex + 1;
                    delimiterIndex = result.IndexOf("|", index);
                    if (delimiterIndex == -1)
                    {
                        break;
                    }

                    string type = result[index..delimiterIndex];
                    index = delimiterIndex + 1;
                    delimiterIndex = result.IndexOf("|", index);
                    if (delimiterIndex == -1)
                    {
                        break;
                    }

                    string id = result[index..delimiterIndex];
                    index = delimiterIndex + 1;
                    if ((index + length) >= result.Length)
                    {
                        break;
                    }

                    string content = result.Substring(index, length);
                    index += length;
                    if (result.Substring(index, 1) != "|")
                    {
                        break;
                    }

                    index++;

                    content = content.Replace($"/{reportServer}/", $"{proxyUrl}/{reportServer}/", StringComparison.InvariantCultureIgnoreCase);
                    if (content.Contains("./ReportViewer.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        content = content.Replace("./ReportViewer.aspx", $"{proxyUrl}/{reportServer}/Pages/ReportViewer.aspx", StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        content = content.Replace("ReportViewer.aspx", $"{proxyUrl}/{reportServer}/Pages/ReportViewer.aspx", StringComparison.InvariantCultureIgnoreCase);
                    }

                    builder.Append(string.Format("{0}|{1}|{2}|{3}|", content.Length, type, id, content));
                }

                result = builder.ToString();
            }
            else
            {
                result = result.Replace($"/{reportServer}/", $"{proxyUrl}/{reportServer}/", StringComparison.InvariantCultureIgnoreCase);

                if (result.Contains("./ReportViewer.aspx", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = result.Replace("./ReportViewer.aspx", $"{proxyUrl}/{reportServer}/Pages/ReportViewer.aspx", StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    result = result.Replace("ReportViewer.aspx", $"{proxyUrl}/{reportServer}/Pages/ReportViewer.aspx", StringComparison.InvariantCultureIgnoreCase);
                }
            }

            response.Headers.Remove("Content-Length");
            response.Headers.Add("Content-Length", new string[] { System.Text.Encoding.UTF8.GetByteCount(result).ToString() });

            await response.WriteAsync(result);
        }

        /// <summary>
        /// Creates an HttpClient with default credentials and automatic decompression.
        /// </summary>
        /// <returns>An HttpClient with default credentials and automatic decompression.</returns>
        private HttpClient CreateHttpClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseDefaultCredentials = true,
            };

            if (httpClientHandler.SupportsAutomaticDecompression)
            {
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            this.OnHttpClientHandlerCreate(ref httpClientHandler);

            return new HttpClient(httpClientHandler);
        }

        /// <summary>
        /// This method is used to create an HttpClientHandler object.
        /// </summary>
        partial void OnHttpClientHandlerCreate(ref HttpClientHandler handler);

        /// <summary>
        /// This method is used to modify the HttpRequestMessage before sending a report request.
        /// </summary>
        partial void OnReportRequest(ref HttpRequestMessage requestMessage);
    }
}
