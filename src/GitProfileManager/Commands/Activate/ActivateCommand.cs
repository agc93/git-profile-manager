using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GitProfileManager.Services;
using Spectre.Cli;

namespace GitProfileManager.Commands.Activate
{
    [Description("Activates a profile")]
    public class ActivateCommand : Command<ActivationSettings>
    {
        public ActivateCommand(IGitConfigService command, IGitProfileStore store) {
            Service = command;
            Store = store;
        }

        public IGitConfigService Service { get; private set; }
        public IGitProfileStore Store { get; private set; }

        public override int Execute(CommandContext context, ActivationSettings settings)
        {
            var profile = Store.ReadProfile(settings.ProfileName);
            var list = new List<bool>();
            var results = profile.Select(c => Service.SetValue(c.Key, c.Value, settings.ApplyGlobally)).ToList();
            if (results.All(r => r)) {
                Console.WriteLine($"All configuration from {settings.ProfileName} profile applied successfully");
                return 0;
            }
            if (results.Any(r => r)) {
                Console.WriteLine($"Some configuration items were not applied successfully. You may need to manually adjust your configuration");
                return 1;
            }
            Console.WriteLine($"Activating profile {settings.ProfileName} was unsuccessful. Check that you are in a valid repository and try again!");
            return 2;
        }
    }
}