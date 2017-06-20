using System;
using Spectre.CommandLine;

namespace GitProfileManager
{
    public sealed class GitCommandApp : CommandAppBase<CommandAppSettings>
    {
        public GitCommandApp() 
            : this(new CommandAppSettings())
        {
        }

        internal GitCommandApp(CommandAppSettings settings) : base(settings) {}

        protected override void Initialize()
        {
        }

        public void RegisterCommand<TCommand>()
            where TCommand : ICommand
        {
            RegisterCommand(typeof(TCommand));
        }
    }
}