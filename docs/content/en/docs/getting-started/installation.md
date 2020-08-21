---
title: "GPM Installation"
linkTitle: "Installation"
weight: 21
description: >
  How to install Git Profile Manager
---

Installing Git Profile Manager is simple and easy, and supports a huge range of platforms.

Currently installers are provided for: Windows 10, Ubuntu (18.04), RPM-based Linux distros and Debian/Ubuntu.

You can also manually install on any of the above, as well as macOS (unsupported).

## .NET Core Global Tool

GPM is now also shipped as a .NET Core *Global Tool*. This means, you can run the following command to install GPM and the .NET Core CLI should make the `gpm` command available in your environment.

```bash
dotnet tool install -g git-profile-manager
```

Note that it's possible (especially on non-Windows platforms) that the global .NET Core tools location is not on your `PATH` so you may need to manually add it (or just install GPM using one of the below methods).

## Linux

### Debian, Ubuntu

First, you need to install some dependencies:

```bash
# for Debian 8
apt-get install -y libicu52 libunwind8
# for Ubuntu 18.04
apt-get install -y libicu55 libunwind8
```

Now, just download the `deb` for your distro and install it with:

```bash
dpkg -i git-profile-manager*.deb
```

This will install the app to `/usr/lib/git-profile-manager` and automatically add `gpm` to your `PATH`.

Just run `gpm` to get the help.

### Fedora

Download the `rpm` for Fedora and install it with:

```bash
dnf install git-profile-manager*.rpm
```

> Despite the naming, this package should work fine for most modern Fedora versions

This will install the app to `/usr/lib/git-profile-manager` and automatically add `gpm` to your `PATH`.

Just run `gpm` to get the help.

### RHEL, CentOS

First, install dependencies:

```bash
yum install -y libicu libunwind
```

Now, download the `rpm` for your flavour and install it with:

```bash
rpm -i git-profile-manager*.rpm
```

This will install the app to `/usr/lib/git-profile-manager` and automatically add `gpm` to your `PATH`.

Just run `gpm` to get the help.

## Windows

Chocolatey users can quickly install Git Profile Manager using the `git-profile-manager.install` package:

```powershell
choco install git-profile-manager.install
```

This will install the app to the default Chocolatey location and should automatically add a `gpm` alias to your PATH.

> You can also download the release package, extract it somewhere and add that location to your `PATH` manually.

Just run `gpm` to get the help.

## macOS

At this time, there is no automated install available for macOS (since building for macOS requires a Mac and I don't have one).

MacOS users can download the relevant archive, extract it to a directory (say `/Applications/git-profile-manager` for example) and add that folder to your `PATH`. You can then run `gpm` from a terminal to run the app.

> Note that the macOS version is largely untested and may be somewhat unstable. Please report issues and bugs on GitHub.

## .NET Runtime

If you're on another platform, GPM _might_ still work! If you can install the .NET Core runtime on your system (this is bundled with the SDK if you're on a supported OS), you should be able to use it to run GPM from the `dotnet-any` package. The main executable is the `git-profile-manager.dll` file:

```bash
dotnet git-profile-manager.dll
```