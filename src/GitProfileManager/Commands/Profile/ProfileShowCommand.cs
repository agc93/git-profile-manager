using System;
using System.ComponentModel;
using System.Linq;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    [Description("Show configuration from a saved profile")]
    public class ProfileShowCommand : Command<ProfileShowCommand.Settings>
    {
        public ProfileShowCommand(IGitProfileStore store)
        {
            Store = store;
        }

        public IGitProfileStore Store { get; }

        public override int Execute(Settings settings, ILookup<string, string> remaining)
        {
            if (string.IsNullOrWhiteSpace(settings.ProfileName)) {
                Console.Error.WriteLine("No profile name provided!");
                return 400;
            }
            var profile = Store.ReadProfile(settings.ProfileName);
            if (profile == null) {
                Console.WriteLine($"Could not find profile '{settings.ProfileName}'! Does it exist?");
                return 404;
            }
            Console.WriteLine($"Profile '{settings.ProfileName}':");
            if (profile.Any()) {
                foreach (var config in profile)
                {
                    Console.WriteLine($"- {config.Key}={config.Value}");
                }
            }
            else {
                Console.WriteLine(" <none>");
            }
            return 0;
        }

        public sealed class Settings : ProfileCommandSettings
        {
        }
        
    }
}