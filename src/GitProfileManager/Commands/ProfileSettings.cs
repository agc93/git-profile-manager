using System.ComponentModel;
using Spectre.CommandLine;
using Spectre.CommandLine.Annotations;

namespace GitProfileManager.Commands
{
    [Description("Commands for working with Git profiles")]
    public abstract class ProfileSettings
    {
        [Argument(0, argumentName: "[PROFILE]")]
        [Description("The profile name")]
        public virtual string ProfileName {get;set;}
    }
}