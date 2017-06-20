using System;
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
                    .AsSelf()
                );
            using (var app = new DependencyInjectionApp(services, disableAutoRegistration: true))
            {
                // Set additional information.
                app.SetTitle("Git Profile Manager");
                app.SetHelpText("Create, manage and activate git profiles for multiple projects");

                /*// Register commands. */
                app.RegisterCommand<Commands.Profile.ProfileCommand>();
                app.RegisterCommand<Commands.Activate.ActivateCommand>();

                // Run the application.
                return app.Run(args);
            }
        }
    }
}
