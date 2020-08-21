using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Cli;

namespace GitProfileManager.Composition
{
    public class DependencyInjectionRegistrar : ITypeRegistrar
    {
        private readonly IServiceCollection _services;
        // private readonly bool _useTransient;

        public DependencyInjectionRegistrar(IServiceCollection services)
        {
            _services = services;
            // _useTransient = useTransient;
        }
        public ITypeResolver Build()
        {
            return new DependencyInjectionResolver(_services);
        }

        public void Register(Type service, Type implementation)
        {
            _services.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            _services.AddSingleton(service, implementation);
        }
    }
}