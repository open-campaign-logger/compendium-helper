// <copyright file="MainLayout.razor.cs" company="Jochen Linnemann - IT-Service">
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

namespace CampaignKit.Compendium.Helper.Shared
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;

    /// <summary>
    /// Code behind class for MainLayou.razor.
    /// </summary>
    public partial class MainLayout
    {
        /// <summary>
        /// Gets or sets the ContextMenuService.
        /// </summary>
        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        /// <summary>
        /// Gets or sets the DialogService.
        /// </summary>
        [Inject]
        protected DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the JSRuntime for JS interop.
        /// </summary>
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Gets or sets the NavigationManager.
        /// </summary>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the NotificationService.
        /// </summary>
        [Inject]
        protected NotificationService NotificationService { get; set; }

        /// <summary>
        /// Gets or sets the TooltipService.
        /// </summary>
        [Inject]
        protected TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sidebar is expanded.
        /// </summary>
        protected bool SidebarExpanded { get; set; } = true;

        /// <summary>
        /// Toggle the sidebarExpanded flag.
        /// </summary>
        protected void SidebarToggleClick()
        {
            this.SidebarExpanded = !this.SidebarExpanded;
        }
    }
}
