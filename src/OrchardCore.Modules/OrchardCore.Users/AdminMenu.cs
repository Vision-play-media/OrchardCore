using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Users.Drivers;

namespace OrchardCore.Users
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

            builder.Add(S["Security"], NavigationConstants.AdminMenuSecurityPosition, security => security
                    .AddClass("security").Id("security")
                        .Add(S["Users"], "am-" + S["Users"], users => users
                            .AddClass("users").Id("users")
                            .Action("Index", "Admin", "OrchardCore.Users")
                            .Permission(Permissions.ManageUsers)
                            .LocalNav()
                         )
                        .Add(S["Settings"], settings => settings
                            .Add(S["Login"], "am-" + S["Login"], login => login
                                .Permission(Permissions.ManageUsers)
                                .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = LoginSettingsDisplayDriver.GroupId })
                                .LocalNav()
                                )
                            )
                       );

            return Task.CompletedTask;
        }
    }

    [Feature("OrchardCore.Users.ChangeEmail")]
    public class ChangeEmailAdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public ChangeEmailAdminMenu(IStringLocalizer<ChangeEmailAdminMenu> localizer)
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
                        .Add(S["Email"], "am-" + S["Email"], registration => registration
                            .Permission(Permissions.ManageUsers)
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = ChangeEmailSettingsDisplayDriver.GroupId })
                            .LocalNav()
                        )));

            return Task.CompletedTask;
        }
    }

    [Feature("OrchardCore.Users.Registration")]
    public class RegistrationAdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public RegistrationAdminMenu(IStringLocalizer<RegistrationAdminMenu> localizer)
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
                        .Add(S["Registration"], "am-" + S["Registration"], registration => registration
                            .Permission(Permissions.ManageUsers)
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = RegistrationSettingsDisplayDriver.GroupId })
                            .LocalNav()
                        )));

            return Task.CompletedTask;
        }
    }

    [Feature("OrchardCore.Users.ResetPassword")]
    public class ResetPasswordAdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public ResetPasswordAdminMenu(IStringLocalizer<ResetPasswordAdminMenu> localizer)
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
                        .Add(S["Reset password"], "am-" + S["Reset password"], password => password
                            .Permission(Permissions.ManageUsers)
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = ResetPasswordSettingsDisplayDriver.GroupId })
                            .LocalNav()
                        )));

            return Task.CompletedTask;
        }
    }
}
