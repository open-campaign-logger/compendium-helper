using CampaignKit.Compendium.Core.Configuration;
using Microsoft.AspNetCore.Components;

namespace CampaignKit.Compendium.Helper.Pages
{
    public partial class SourceDataSetProperties
    {
        [Parameter]
        public SourceDataSet SourceData { get; set; } = new SourceDataSet();
    }
}
