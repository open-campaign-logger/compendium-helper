using Microsoft.AspNetCore.Components;

namespace CampaignKit.Compendium.Helper.Dialogs
{
    public partial class RemoveLabelsDialog
    {
        [Parameter]
        public List<string> AllLabels { get; set; }
    }
}
