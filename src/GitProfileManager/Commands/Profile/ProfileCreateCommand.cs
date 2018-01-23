using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    [Description("Create a new (blank) profile")]
    public class ProfileCreateCommand : Command<ProfileCreateCommand.Settings>
    {
        public ProfileCreateCommand(IGitProfileStore store, ICommandFileService fileService)
        {
            Store = store;
            FileService = fileService;
        }

        public IGitProfileStore Store { get; private set; }
        public ICommandFileService FileService { get; private set; }

        public override int Execute(Settings settings, ILookup<string, string> unmapped)
        {
            var source = !string.IsNullOrWhiteSpace(settings.SourceProfileName);
            var cmds = new Dictionary<string, string>();
            if (source) {
                var profile = Store.ReadProfile(settings.SourceProfileName);
                cmds = profile;
            }
            var d = Store.WriteProfile(settings.ProfileName, cmds);
            if (d) {
                Console.WriteLine($"Succesfully created '{settings.ProfileName}' profile {(source ? "from " + settings.SourceProfileName : string.Empty)}");
                Console.WriteLine($"Activate it using 'git-profile-manager activate {settings.ProfileName}'");
                return 200;
            }
            Console.WriteLine($"Error encountered while creating profile!");
            return 500;
        }

        public sealed class Settings : ProfileCommandSettings
        {
            [CommandOption("--from")]
            [Description("An existing profile to base the new profile on (essentially duplicates the existing profile)")]
            public string SourceProfileName {get;set;}
        }
    }
}