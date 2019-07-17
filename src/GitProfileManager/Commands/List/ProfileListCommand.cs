using System;
using System.Linq;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.List
{
    [System.ComponentModel.Description("Lists available profiles")]
    public class ProfileListCommand : Command<ProfileListCommand.Settings>
    {
        public ProfileListCommand(IGitProfileStore store)
        {
            Store = store;
        }

        public IGitProfileStore Store { get; private set; }

        public override int Execute(Settings settings, ILookup<string, string> unmapped)
        {
            var profiles = Store.GetProfiles();
            Console.WriteLine("Currently stored profiles: ");
            if (profiles.Any()) {
                foreach (var profile in profiles)
                {
                    Console.WriteLine($"- {profile}");
                }
            }
            else {
                Console.WriteLine(" <none>");
            }
            return 0;
        }

        public sealed class Settings : CommandSettings {

        }
    }
}