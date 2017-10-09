using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.CommandLine;

namespace GitProfileManager
{
    internal class DependencyInjectionResolver : ITypeResolver
    {
        public DependencyInjectionResolver(IServiceCollection services)
        {
            Services = services;
        }
        internal IServiceCollection Services {get;set;}

        public object Resolve(Type type)
        {
            return (Services.BuildServiceProvider()).GetService(type);
        }
    }
}