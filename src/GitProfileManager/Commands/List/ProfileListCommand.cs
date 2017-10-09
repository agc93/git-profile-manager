using System;
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

        public override int Run(Settings settings)
        {
            var profiles = Store.GetProfiles();
            Console.WriteLine("Currently stored profiles: ");
            foreach (var profile in profiles)
            {
                Console.WriteLine($"- {profile}");
            }
            return 0;
        }

        public sealed class Settings {

        }
    }
}