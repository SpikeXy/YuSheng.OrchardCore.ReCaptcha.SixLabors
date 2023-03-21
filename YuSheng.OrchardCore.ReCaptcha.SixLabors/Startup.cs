using Fluid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.ReCaptcha.Users.Handlers;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using OrchardCore.Settings.Deployment;
using OrchardCore.Users.Events;
using System;
using YuSheng.OrchardCore.ReCaptcha.SixLabors;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Configuration;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Drivers;
using YuSheng.OrchardCore.ReCaptcha.SixLabors.Services;

namespace YuSheng.OrchardCore.ReCaptcha.SixLabors
{

    public class Startup : StartupBase
    {
        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            builder.UseSession();
           // routes.MapAreaControllerRoute(
           //    name: "SixLaborsCaptcha",
           //    areaName: "JMBCore.Project.Common.JMBBackend",
           //    pattern: "JMBBackend/{controller=Home}/{action=Index}/{Id=UrlParameter.Optional}",
           //    defaults: new { controller = "Home", action = "Index" }
           //);
        }
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddTransient<IConfigureOptions<SixLaborsCaptchaSettings>, SixLaborsCaptchaSettingsConfiguration>();
            services.AddTransient<SixLaborsCaptchaService>();
            services.AddScoped<IDisplayDriver<ISite>, SixLaborsCaptchaSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IPermissionProvider, Permissions>();

            services.AddScoped<IRegistrationFormEvents, RegistrationFormEventHandler>();
            services.AddScoped<ILoginFormEvent, LoginFormEventEventHandler>();
            services.AddScoped<IPasswordRecoveryFormEvents, PasswordRecoveryFormEventEventHandler>();
            services.Configure<MvcOptions>((options) =>
            {
                options.Filters.Add(typeof(SixLaborsCaptchaLoginFilter));
            });
        }
    }

}
