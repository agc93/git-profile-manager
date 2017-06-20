using System.ComponentModel;
using Spectre.CommandLine;

namespace GitProfileManager.Commands
{
    public abstract class ProfileSettings
    {
        [Argument("<PROFILE>", Order = 0)]
        [Description("The profile name")]
        public virtual string ProfileName {get;set;}
    }
}