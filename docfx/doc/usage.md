# Usage Guide

> [!NOTE]
> The instructions below are based on the Linux version (which uses `gpm`).
> Windows/macOS users should use `git-profile-manager` instead (or add an alias).

## Getting help

You can always run `gpm --help` to get info on the top-level commands available. Note that each command also has it's own `--help` option for easy assistance.

## Activate

The activate command is the essential command for `gpm`. Activating a profile automatically applies all configuration options stored in the profile, applied to the current repository.

You can also add the `-g` or `--global` flag to apply a profile globally (use with care).

#### Example

```bash
gpm activate work-profile
```

## Deactivate

Deactivate is the opposite of activate and will unset any configuration options stored in the profile. Like `activate`, this applies to the current repository by default, but the `--global` flag will unset these items globally (use with ***extreme* caution**).

#### Example

```bash
gpm deactivate my-oss-project
```

## List

The list command simply lists all the currently saved Git profiles (from the user's `~/.gitprofile`).

#### Example

```bash
gpm profile list
```

## Profile

The profile command is used to managed saved Git profiles, and includes a number of subcommands.

### Show

Show will simply print the configuration items stored in a specific profile.

#### Example

```bash
gpm profile show personal
```

### Create

Create will create a new profile. By default, this profile is blank (i.e. includes no configuration options), but you can optionally provide a source profile for GPM to duplicate.

#### Example

```bash
gpm profile create --from personal work
```

### Delete

Delete will delete the specified profile from your saved profiles completely. By default, you will be prompted to perform this command, but this can be skipped using the `--non-interactive` option.

#### Example

```bash
gpm profile delete work-profile
```

### Edit

Edit allows you to edit already existing profiles (i.e. to add new configuration options to a profile). The profile will be created if it doesn't exist.

Configuration options are specified as key-value pairs, similar to `git config`, separated by an '`=`' symbol. You can remove a configuration option using the `--rm` option

#### Example

```bash
gpm profile edit work-profile commit.gpgsign=true
gpm profile edit work-profile gpg.signingkey=123ABCDEF --rm
```

### Export

If you'd like to use your profiles outside of Git Profile Manager, you can use this option to export your profile into a file you can then copy/run/move as you see fit. The exported file is a plain text file with all the `git config` commands for your profile, one command per line.

This file can then be run (through Bash or PowerShell) or used however you like.

#### Example

```bash
gpm profile export work-profile command-file.txt
```

### Import

The opposite operation to export, import accepts a simple command file and creates a profile from it. GPM will read through the input file and create a profile with all of the Git configuration items found in the file. As such, this can be used to import not just from exported GPM profiles, but also from simple shell scripts you may already be using.

The new profile will be named with the input file's name, or this can be overriden with the `--profile-name` option.

> [!NOTE]
> The file parsing logic in GPM is quite "dumb" so it will not, for example, process any of the logic contained in the command file.

#### Example

```bash
gpm profile import path/to/file.sh -n new-profile
```