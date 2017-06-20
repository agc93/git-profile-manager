using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    public class ProfileImportCommand : Command<ProfileImportCommand.Settings>
    {
        public ProfileImportCommand(IGitProfileStore store, ICommandFileService fileService) : base("import")
        {
            Store = store;
            FileService = fileService;
        }

        public IGitProfileStore Store { get; private set; }
        public ICommandFileService FileService { get; private set; }

        public override int Run(Settings settings)
        {
            if (settings.AsConfig) {
                Console.WriteLine("Sorry, this functionality is not yet available :(");
                return 501;
            }
            var file = new FileInfo(settings.CommandFile);
            if (!file.Exists) {
                Console.Error.WriteLine($"Could not find file at {settings.CommandFile}");
                return 404;
            }
            var name = GetProfileName(settings);
            var cmds = FileService.ReadFromFile(file);
            var d = Store.WriteProfile(name, cmds);
            if (d) {
                Console.WriteLine($"Succesfully created '{name}' profile from '{file.FullName}'!");
                Console.WriteLine($"Activate it using 'git-profile-manager activate {name}'");
                return 200;
            }
            Console.WriteLine($"Error encountered while creating profile from '{file.FullName}'!");
            return 500;
        }

        private string GetProfileName(Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.ProfileName)) {
                var fi = new FileInfo(settings.CommandFile);
                return fi.Name.Replace(fi.Extension, string.Empty).Trim().Trim('.');
            }
            return settings.ProfileName;
        }

        public sealed class Settings
        {
            [Argument("<FILE>", Order = 0)]
            [Description("A file of git commands to create a profile from.")]
            public string CommandFile { get; set; }

            [Option("-n|--profile-name")]
            [Description("Name of the profile to create. Defaults to the input file name")]
            public string ProfileName {get;set;}

            [Option("--from-config")]
            [Description("Read commands from config, rather than command file")]
            public bool AsConfig { get; set; }
        }
    }
}