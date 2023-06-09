﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Drivers;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            S = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            builder
                .Add(S["Security"], security => security
                    .Add(S["Settings"], settings => settings
                        .Add(S["SixLaborsCaptcha"], S["SixLaborsCaptcha"].PrefixPosition(), registration => registration
                            .Permission(Permissions.SixLaborsCaptcha)
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = SixLaborsCaptchaSettingsDisplayDriver.GroupId })
                            .LocalNav()
                        )));

            return Task.CompletedTask;
        }
    }
}
