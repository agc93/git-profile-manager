using System.ComponentModel;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Activate
{
    public sealed class ActivationSettings
    {
        [Argument("<PROFILE>", Order = 0)]
        [Description("The Git profile to activate or deactivate")]
        public string ProfileName { get; set; }

        [Option("-g|--global")]
        [Description("Applies the profile globally, instead of the current repository.")]
        public bool ApplyGlobally { get; set; } = false;
    }
}