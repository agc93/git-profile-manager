using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GitProfileManager.Commands.Activate;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Deactivate
{
    [Description("Deactivates a profile")]
    public class DeactivateCommand : Command<ActivationSettings>
    {
        public DeactivateCommand(IGitConfigService command, IGitProfileStore store) {
            Service = command;
            Store = store;
        }

        public IGitConfigService Service { get; private set; }
        public IGitProfileStore Store { get; private set; }

        public override int Execute(ActivationSettings settings, ILookup<string, string> unmapped)
        {
            //Console.WriteLine($"Service loaded: {Service != null}");
            var profile = Store.ReadProfile(settings.ProfileName);
            var list = new List<bool>();
            var results = profile.Select(c => Service.UnsetValue(c.Key, c.Value, settings.ApplyGlobally)).ToList();
            if (results.All(r => r)) {
                Console.WriteLine($"All configuration from {settings.ProfileName} profile reversed successfully");
                return 0;
            }
            if (results.Any(r => r)) {
                Console.WriteLine($"Some configuration items were not reversed successfully. You may need to manually adjust your configuration");
                return 1;
            }
            Console.WriteLine($"Deactivating profile {settings.ProfileName} was unsuccessful. Check that you are in a valid repository and try again!");
            return 2;
        }
    }
}