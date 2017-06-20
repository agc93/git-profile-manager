using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Spectre.CommandLine;

namespace GitProfileManager
{
    public class DependencyInjectionApp : CommandAppBase<CommandAppSettings>
    {
        public IServiceCollection Services { get; private set; }

        public DependencyInjectionApp()
            : this(new ServiceCollection())
        {
        }

        public DependencyInjectionApp(IServiceCollection services, bool disableAutoRegistration = false)
            : this(new CommandAppSettings { Resolver = new InjectionResolver(services) })
        {
            if (!disableAutoRegistration) {
                var commands = services.Where(s => s.ImplementationType.IsAssignableTo(typeof(ICommand))).ToList();
                foreach (var command in commands)
                {
                    RegisterCommand(command.ImplementationType);
                }
            }
            Services = services;
        }

        internal DependencyInjectionApp(CommandAppSettings settings) : base(settings)
        {
        }

        protected override void Initialize()
        {
            
        }

        public void RegisterCommand<TCommand>()
            where TCommand : class, ICommand
        {
            Services.AddTransient<TCommand>();
            RegisterCommand(typeof(TCommand));
        }
    }
}