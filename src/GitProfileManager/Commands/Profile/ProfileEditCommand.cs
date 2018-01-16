using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    [Description("Edit an existing profile to add or remove configuration items")]
    public class ProfileEditCommand : Command<ProfileEditCommand.Settings>
    {
        public ProfileEditCommand(IGitProfileStore store)
        {
            Store = store;
        }

        public IGitProfileStore Store { get; private set; }

        public override int Execute(Settings settings, ILookup<string, string> unmapped)
        {
            var profile = Store.ReadProfile(settings.ProfileName) ?? new Dictionary<string, string>();
            var config = settings.RawConfig.Split('=');
            if (profile.ContainsKey(config[0]))
            {
                profile.Remove(config[0]);
            }
            if (!settings.Remove)
            {
                profile.Add(config[0], config[1]);
            }
            var result = Store.WriteProfile(settings.ProfileName, profile);
            return result ? 0 : 2;
        }

        public sealed class Settings : ProfileCommandSettings
        {
            [CommandArgument(0, "<NAME>")]
            [Description("The profile to add a new configuration to. Will be created if it does not exist")]
            public override string ProfileName { get; set; }

            [CommandArgument(1, "<CONFIG>")]
            [Description("The config value to add to the profile, separated by an '=' symbol.")]
            public string RawConfig { get; set; }

            [CommandOption("-r|--rm")]
            [Description("Removes the given configuration item from the profile, instead of adding it.")]
            public bool Remove { get; set; }
        }
    }
}