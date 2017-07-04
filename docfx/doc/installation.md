# GPM Installation

Installing Git Profile Manager is simple and easy, and supports a huge range of platforms.

Currently installers are provided for: Windows 10, Ubuntu (16.04, 14.04), RHEL 7, Debian 8, and CentOS 7.

You can also manually install on any of the above, as well as macOS 10.12 *Sierra*.

## Linux

### Debian, Ubuntu

First, you need to install some dependencies:

```bash
apt-get install -y libicu52 libunwind8
```

> [!WARNING]
> There is a [known issue](https://github.com/dotnet/cli/issues/2018#issuecomment-221633831) with .NET Core packaging that means you will need to add the `trusty-security` repositories to get `libicu52` on Ubuntu 16.04

Now, just download the `deb` for your distro and install it with:

```bash
dpkg -i git-profile-manager*.deb
```

This will install the app to `/usr/lib/git-profile-manager` and automatically add `gpm` to your `PATH`.

Just run `gpm` to get the help.

### RHEL, CentOS

First, install dependencies:

```bash
yum install -y libicu libunwind
```

Now, download the `rpm` for your flavour and install it with:

```bash
rpm -i git-profile-manager*.deb
```

This will install the app to `/usr/lib/git-profile-manager` and automatically add `gpm` to your `PATH`.

Just run `gpm` to get the help.

## Windows

Chocolatey users can quickly install Git Profile Manager using the `git-profile-manager.install` package:

```powershell
choco install git-profile-manager.install
```

This will install the app to the default Chocolatey location and should automatically add `git-profile-manager.exe` to your PATH.

> [!NOTE]
> You can also download the release package, extract it somewhere and add that location to your `PATH` manually.

Just run `git-profile-manager` to get the help.

> [!TIP]
> You can use PowerShell's `Set-Alias` command to alias `git-profile-manager` to `gpm` to cut down on typing

## macOS

At this time, there is no automated install available for macOS (since building for macOS requires a Mac and I don't have one).

10.12/*Sierra* users can download the release package, extract it to a directory (say `/Applications/git-profile-manager` for example) and add that folder to your `PATH`. You can then run `git-profile-manager` from a terminal to run the app.

> [!WARNING]
> Note that the macOS version is largely untested and may be somewhat unstable. Please report issues and bugs on GitHub.

> [!TIP]
> Add `alias gpm=git-profile-manager` to your `~/.bashrc` to cut down on typing

## Docker

We do also provide a Docker image, based on CentOS 7, that you can use to quickly test out GPM without installing it on your local machine. Note that this is not a recommended way of running GPM as it requires you to bind your current directory into the container using volumes, and profiles won't be shared with the host unless you also bind `~/.gitprofiles` into the container.

```bash
# Basic run
docker run -it agc93/gpm
# Advanced usage (for Bash)
docker run -it -v $PWD:/app -v $HOME/.gitprofiles:/home/app/.gitprofiles -w /app agc93/gpm
```