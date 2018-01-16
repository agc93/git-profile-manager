﻿using System;
using GitProfileManager.Composition;
using GitProfileManager.Services;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Spectre.CommandLine;

namespace GitProfileManager
{
    class Program
    {
        public static int Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<IGitConfigService, GitCommandService>()
                .AddSingleton<GitCommandRunner>()
                .AddTransient<IGitProfileStore, FileProfileStore>()
                .AddSingleton<ICommandFileService, CommandFileService>()
                .Scan(s => s.FromAssemblyOf<Program>()
                    .AddClasses(f => f.AssignableTo(typeof(Command<>)))
                    .AddClasses(f => f.AssignableTo(typeof(Commands.ProfileCommandSettings)))
                    .AsSelf()
                );
            var app = new CommandApp(new DependencyInjectionRegistrar(services));
            app.Configure(config =>
            {
                // Set additional information.
                config.SetApplicationName("Git Profile Manager");
                // app.SetHelpText("Create, manage and activate git profiles for multiple projects");

                /*// Register commands. */
                config.AddCommand<Commands.Activate.ActivateCommand>("activate");
                config.AddCommand<Commands.Deactivate.DeactivateCommand>("deactivate");
                config.AddCommand<Commands.List.ProfileListCommand>("list");
                config.AddCommand<Commands.ProfileSettings>("profile", profile =>
                {
                    profile.SetDescription("Commands for working with Git profiles");
                    profile.AddCommand<Commands.Profile.ProfileCreateCommand>("create");
                    profile.AddCommand<Commands.Profile.ProfileImportCommand>("import");
                    profile.AddCommand<Commands.Profile.ProfileDeleteCommand>("delete");
                    profile.AddCommand<Commands.Profile.ProfileEditCommand>("edit");
                    profile.AddCommand<Commands.Profile.ProfileExportCommand>("export");
                });

                // Run the application.

            });
            return app.Run(args);
        }
    }
}
