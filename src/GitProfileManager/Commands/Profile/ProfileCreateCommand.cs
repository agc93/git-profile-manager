using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    public class ProfileCreateCommand : Command<ProfileCreateCommand.Settings>
    {
        public ProfileCreateCommand(IGitProfileStore store, ICommandFileService fileService) : base("create")
        {
            Store = store;
            FileService = fileService;
        }

        public IGitProfileStore Store { get; private set; }
        public ICommandFileService FileService { get; private set; }

        public override int Run(Settings settings)
        {
            var source = !string.IsNullOrWhiteSpace(settings.SourceProfileName);
            var cmds = new Dictionary<string, string>();
            if (source) {
                var profile = Store.ReadProfile(settings.SourceProfileName);
                cmds = profile;
            }
            var d = Store.WriteProfile(settings.Name, cmds);
            if (d) {
                Console.WriteLine($"Succesfully created '{settings.Name}' profile {(source ? "from " + settings.SourceProfileName : string.Empty)}");
                Console.WriteLine($"Activate it using 'git-profile-manager activate {settings.Name}'");
                return 200;
            }
            Console.WriteLine($"Error encountered while creating profile!");
            return 500;
        }

        public sealed class Settings
        {
            [Argument("<NAME>", Order = 0)]
            [Description("Name of the profile to create.")]
            public string Name { get; set; }

            [Option("--from")]
            [Description("An existing profile to base the new profile on (essentially duplicates the existing profile)")]
            public string SourceProfileName {get;set;}
        }
    }
}