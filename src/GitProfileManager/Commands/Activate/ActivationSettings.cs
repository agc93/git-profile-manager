using System.ComponentModel;
using Spectre.Cli;

namespace GitProfileManager.Commands.Activate
{
    public sealed class ActivationSettings : CommandSettings
    {
        [CommandArgument(0, "<PROFILE>")]
        [Description("The Git profile to activate or deactivate")]
        public string ProfileName { get; set; }

        [CommandOption("-g|--global")]
        [Description("Applies the profile globally, instead of the current repository.")]
        public bool ApplyGlobally { get; set; } = false;
    }
}