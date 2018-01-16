using System;
using System.ComponentModel;
using System.Linq;
using GitProfileManager.Services;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    [Description("Delete an existing saved profile")]
    public class ProfileDeleteCommand : Command<ProfileDeleteCommand.Settings>
    {
        public ProfileDeleteCommand(IGitProfileStore store)
        {
            Store = store;
        }

        public IGitProfileStore Store { get; private set; }

        public override int Execute(Settings settings, ILookup<string, string> unmapped)
        {
            var profile = Store.ReadProfile(settings.ProfileName);
            if (profile == null) return 404;
            bool confirm = false;
            if (!settings.NonInteractive) {
                while(!confirm) {
                    Console.Write($"{Environment.NewLine}This will complete remove the '{settings.ProfileName}' profile. Are you sure? [y/n]");
                    int key;
                    try {
                        var keyInfo = Console.ReadKey();
                        //Console.Write(keyInfo.KeyChar);
                        key = ((int)keyInfo.KeyChar);
                    } catch (InvalidOperationException) {
                        key = Console.Read();
                    } catch {
                        Console.WriteLine();
                        Console.Error.WriteLine("Could not confirm on current terminal. Try passing --non-interactive to delete without confirmation.");
                        return 412;
                    }
                    Console.WriteLine();
                    if (key == 'n' || key == 'N') return 3;
                    confirm = key == 'y' || key == 'Y';
                }
            }
            var del = Store.DeleteProfile(settings.ProfileName);
            if (del) {
                Console.WriteLine($"Removed '{settings.ProfileName}' profile from store!");
                return 0;
            }
            Console.Error.WriteLine("Error deleting profile from store!");
            return 1;
        }

        public sealed class Settings : ProfileCommandSettings
        {
            [CommandOption("--non-interactive")]
            [Description("Do not prompt for user input or confirmations.")]
            public bool NonInteractive { get; set; }

        }
    }
}