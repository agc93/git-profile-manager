using System.ComponentModel;
using Spectre.CommandLine;

namespace GitProfileManager.Commands
{
    public class ProfileSettings {}
    public class ProfileCommandSettings : ProfileSettings
    {
        [CommandArgument(0, "<PROFILE>")]
        [Description("The profile name")]
        public virtual string ProfileName {get;set;}
    }
}