---
title: "Developer Guide"
linkTitle: "Development"
weight: 90
description: >
  An introduction to GPM's design and how to contribue.
---

## Building

Building this project is super-simple, and only needs the .NET Core SDK installed: run `dotnet tool restore` then `dotnet cake`.

> The documentation is built separately at this time as it's a temporary solution only.

### Packaging

To do a complete build of the tool *and* build all packages, you will need to have Docker installed and available on your host (and accessible to the user running the build script). The build script will complete all the packaging steps in a series of Docker containers so this may be a slow process the first time you run it as the relevant images are pulled from the Hub.

## Introduction

GPM is a simple enough codebase that should be pretty easy to work with for veterans and beginners alike. Each of the operations (edit profile, import profile, activation/deactivation etc) is handled inside a single `ICommand<>` implementation, generally with the settings class declared inside or alongside the command class.

These commands are wired up using [Spectre.Cli](https://nuget.org/packages/Spectre.Cli), an awesome command line parser built by Patrik Svensson.

## Dependency Injection

This app includes basic dependency injection support for command implementations. Simply include the types you need in the `ICommand<>`'s constructor, and the app will attempt to resolve this dependency from the DI container. If not found in the DI container, it will attempt to create a new instance (default Spectre.Cli behaviour), but this will only work with a parameterless constructor.

You can see an example of this in the `ActivateCommand` command type.

As with any other DI-controlled app, you must register the dependency first, which is done in `Program.cs` before the app is started.

## Help Information

Help information is automatically generated from the attributes in the settings class, especially the `CommandOption`/`CommandArgument` attribute (in the `Spectre.Cli` namespace) and the `Description` attribute (in the `System.ComponentModel` namespace). The help option is **always** `-h` or `--help`.

## Licensing

GPM is made available under an [MIT License](https://opensource.org/licenses/MIT). That means all contributions will also be licensed under the MIT License and all the conditions and limitations that involves.
