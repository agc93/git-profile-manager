using System;
using System.ComponentModel;
using System.IO;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    public class ProfileExportCommand : Command<ProfileExportCommand.Settings>
    {
        public ProfileExportCommand(IGitProfileStore store, ICommandFileService fileService) : base("export")
        {
            Store = store;
            FileService = fileService;
        }

        public IGitProfileStore Store { get; private set; }
        public ICommandFileService FileService { get; private set; }

        public override int Run(Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.ProfileName)) {
                Console.Error.WriteLine("No profile name provided!");
                return 400;
            }
            if (string.IsNullOrWhiteSpace(settings.FilePath)) {
                Console.Error.WriteLine("No export path provided!");
                return 400;
            }
            var profile = Store.ReadProfile(settings.ProfileName);
            if (profile == null) {
                Console.WriteLine($"Could not find profile '{settings.ProfileName}'! Does it exist?");
                return 404;
            }
            var fi = new FileInfo(settings.FilePath);
            if (fi.Exists && fi.IsReadOnly) {
                Console.Error.WriteLine($"Cannot write to the file at '{fi.FullName}'!");
                return 403;
            }
            var d = FileService.WriteToFile(profile, fi, includeCommand: true);
            if (d) {
                Console.WriteLine($"Wrote '{settings.ProfileName}' profile to command file at {fi.FullName}");
                return 0;
            }
            Console.Error.WriteLine("Error writing profile to command file!");
            return 1;
        }

        public sealed class Settings : ProfileSettings {

            [Argument("<FILE>", Order = 1)]
            [Description("The path to export the profile to")]
            public string FilePath {get;set;}           

        }
    }
}