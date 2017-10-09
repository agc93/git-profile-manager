using System;
using System.Collections.Generic;
using System.ComponentModel;
using GitProfileManager.Services;
using Spectre.CommandLine;
using Spectre.CommandLine.Annotations;

namespace GitProfileManager.Commands.Profile
{
    public class ProfileEditCommand : Command<ProfileEditCommand.Settings>
    {
        public ProfileEditCommand(IGitProfileStore store)
        {
            Store = store;
        }

        public IGitProfileStore Store { get; private set; }

        public override int Run(Settings settings)
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

        public sealed class Settings : ProfileSettings
        {
            [Argument(0, "<NAME>")]
            [Description("The profile to add a new configuration to. Will be created if it does not exist")]
            public new string ProfileName { get; set; }

            [Argument(1, "<CONFIG>")]
            [Description("The config value to add to the profile, separated by an '=' symbol.")]
            public string RawConfig { get; set; }

            [Option("-r|--rm")]
            [Description("Removes the given configuration item from the profile, instead of adding it.")]
            public bool Remove { get; set; }
        }
    }
}