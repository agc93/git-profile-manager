using System.ComponentModel;
using Spectre.Cli;

namespace GitProfileManager.Commands
{
    public class ProfileSettings : CommandSettings {}
    public class ProfileCommandSettings : ProfileSettings
    {
        [CommandArgument(0, "<PROFILE>")]
        [Description("The profile name")]
        public virtual string ProfileName {get;set;}
    }
}