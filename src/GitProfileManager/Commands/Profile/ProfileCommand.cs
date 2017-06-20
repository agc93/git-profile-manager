using System;
using Spectre.CommandLine;

namespace GitProfileManager.Commands.Profile
{
    public class ProfileCommand : ProxyCommand
    {
        public ProfileCommand() : base("profile") {}
        public override void Configure(ICommandRegistrar registrar)
        {
            registrar.Register<ProfileCreateCommand>();
            registrar.Register<ProfileImportCommand>();
            registrar.Register<ProfileDeleteCommand>();
            registrar.Register<ProfileEditCommand>();
            registrar.Register<ProfileExportCommand>();
            registrar.Register<ProfileListCommand>();
        }
    }
}