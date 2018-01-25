# Development Guide

> [!TIP]
> You can check the [Developer Reference](../api/index.md) above for full API source reference, directly from the source code.

## Building

Building this project is super-simple: run `build.ps1` if you're on Windows, or `build.sh` if you're on Linux.

> To build the documentation, you may also need `wkhtmltopdf` installed.

### Packaging

To do a complete build of the tool *and* build all packages, you will need to have Docker installed and available on your host (and accessible to the user running the build script). The build script will complete all the packaging steps in a series of Docker containers so this may be a slow process the first time you run it as the relevant images are pulled from the Hub.

## Introduction

GPM is a simple enough codebase that should be pretty easy to work with for veterans and beginners alike. Each of the operations (edit profile, import profile, activation/deactivation etc) is handled inside a single `ICommand<>` implementation, generally with the settings class declared inside or alongside the command class.

These commands are wired up using [Spectre.CommandLine](https://nuget.org/packages/Spectre.CommandLine), an awesome command line parser built by Patrik Svensson.

## Dependency Injection

This app includes basic dependency injection support for command implementations. Simply include the types you need in the `ICommand<>`'s constructor, and the app will attempt to resolve this dependency from the DI container. If not found in the DI container, it will attempt to create a new instance (default Spectre.CommandLine behaviour), but this will only work with a parameterless constructor.

You can see an example of this in the [ActivateCommand](xref:GitProfileManager.Commands.Activate.ActivateCommand) command type.

As with any other DI-controlled app, you must register the dependency first, which is done in `Program.cs` before the app is started.

## Help Information

Help information is automatically generated from the attributes in the settings class, especially the `CommandOption`/`CommandArgument` attribute (in the `Spectre.CommandLine` namespace) and the `Description` attribute (in the `System.ComponentModel` namespace). The help option is **always** `-h` or `--help`.