using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.CommandLine;

namespace GitProfileManager
{
    internal class InjectionResolver : IResolver
    {
        public InjectionResolver(IServiceCollection services)
        {
            Services = services;
        }
        internal IServiceCollection Services {get;set;}

        public object Resolve(Type type)
        {
            return (Services.BuildServiceProvider()).GetService(type) ?? Activator.CreateInstance(type);
        }
    }
}