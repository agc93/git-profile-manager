# Git Profiles

## What's in a profile?

Essentially a Git "profile" is just a collection of configuration key-value pairs. For example `commit.gpgsign=true` is a single configuration item as is `user.name="Alistair Chapman"`.

## Can multiple profiles be active?

Absolutely! Since profiles are just a configuration collection, you can activate as many profiles as you want! Note that profiles you activate will overwrite any previous configuration values so start generic and go specific.

You can also duplicate profiles by creating a new profile and using the `--from` option to specify an existing profile. You can then edit your new profile without affecting the old one.

## Where are they?

GPM stores profiles in a file in your home directory called `.gitprofiles`. Linux and macOS users may need to show hidden files in their file manager before it's visible.

This file is in YAML format (because YAML is awesome) and stores each profile as a named key with the configuration items included right in the file.

> [!TIP]
> If you need to perform bulk editing, you can always directly edit `~/.gitprofiles`. GPM doesn't mind.

## How do I create/edit/delete profiles?

Profiles can be managed using the `gpm profile` command and all of its subcommands, detailed in full in the [usage documentation](./usage.md).