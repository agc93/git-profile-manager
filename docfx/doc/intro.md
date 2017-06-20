# Git Profile Manager

## Introduction

Git Profile Manager is a simple command line application to manage "profiles" for common Git configurations. For example, if you contribute to multiple repositories or teams and are often needing to change repository-specific (or global) addresses or options.

### History

A little history first: this tool was actually originally just intended to be a simple demo for .NET Core native packaging, but I actually quite liked the idea and built it out properly and have now decided to publish it for everyone to hopefully find useful!

### Technology

Git Profile Manager is built entirely in .NET Core, using the excellent `Spectre.CommandLine` and `YamlDotNet` libraries. Note that since it's using native Git underneath you'll need `git` available and on your `PATH`. Support for running without Git itself may come in a future release.

## Getting Started

To get started, check out [how to install for your platform](./installation.md), read the [usage guide](./usage.md) and check out the [FAQ](./faq.md) for common questions.

## Contributing

This utility is completely open-source and is published [on GitHub](https://github.com/agc93/git-profile-manager). To get started contributing, check out the [contributing guide](./contributing.md) and the [developer guide](./developers.md).