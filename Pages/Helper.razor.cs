using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using CampaignKit.Compendium.Helper.Services;
using CampaignKit.Compendium.Core.Configuration;

namespace CampaignKit.Compendium.Helper.Pages
{
    public partial class Helper
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected DownloadService DownloadService {get; set;}

        [Inject]
        protected CompendiumService CompendiumService {get; set;}

        [Inject]
        protected HtmlService HtmlService {get; set;}

        [Inject]
        protected MarkdownService MarkdownService {get; set;}

        [Inject]
        protected ILogger<Helper> Logger {get; set;}

        // Create a string to hold the html source.
        protected string Html {get; set;} = string.Empty;

        // Create a string to hold the markdown extract.
        protected string Markdown {get; set;} = string.Empty;

        // Create a dictionary to store events and their associated timestamps
        protected Dictionary<DateTime, string> events = new Dictionary<DateTime, string>();

        // Create a list of compendiums to store uploaded data
        protected List<PublicCompendium> PublicCompendiums = new List<PublicCompendium>();

        /// <summary>
        /// Logs an event with a given name and value.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="value">The value of the event.</param>
        protected void Log(string eventName, string value)
        {
            events.Add(DateTime.Now, $"{eventName}: {value}");
        }

        /// <summary>
        /// Logs a change event with the item text from the TreeEventArgs.
        /// </summary>
        /// <param name="args">The TreeEventArgs containing the item text.</param>
        protected void LogChange(TreeEventArgs args)
        {
            Log("Change", $"Item Text: {args.Text}");
        }

        /// <summary>
        /// Logs the expand event of a tree item.
        /// </summary>
        /// <param name="args">The tree expand event arguments.</param>
        protected void LogExpand(TreeExpandEventArgs args)
        {
            if (args.Text is string text)
            {
                Log("Expand", $"Item Text: {text}");
            }
        }

        /// <summary>
        /// Uploads a compendium and creates a tree from it.
        /// </summary>
        /// <returns>
        /// A tree created from the uploaded compendium.
        /// </returns>
        protected async Task UploadComplete(Radzen.UploadCompleteEventArgs args)
        {
            Logger.LogInformation($"Upload complete and converted to string of length {args.RawResponse.Length}.");
            var json = args.RawResponse;
            PublicCompendiums = CompendiumService.LoadCompendiums(json);
        }

        protected override async Task OnInitializedAsync()
        {            
        }
    }
}