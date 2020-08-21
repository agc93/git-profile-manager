---
title: "Frequently Asked Questions"
linkTitle: "FAQ"
weight: 80
---

### But, why?

In short, because I could.

In reality, this tool was originally designed as a simple example of a command-line app to demonstrate .NET Core native publishing, Docker in Cake builds and using Spectre.Cli in CLI apps. As it turns out, I found the app pretty handy and since I'd already put the work in, it made sense to publish it.

### What platforms are supported?

Theoretically, Git Profile Manager is supported anywhere .NET Core is. 

If you already have the .NET Core runtime (i.e. `dotnet`), then you can run GPM.

Even if you don't have .NET Core installed, we build packages for Windows, macOS, RHEL/Fedora/CentOS, and Debian/Ubuntu.

### Can't you do the same thing as this tool with shell scripts?

Yes, you can! And if you like shell scripts, power to you!

There's a couple of reasons I built this out. For one, *real* cross-platform support. While WSL brings Bash to Windows, it's not most developers' primary environment. Second, I think this approach is less fragile. You can sync your user profiles file (`~/.gitprofiles`) onto any mcahine and you get the same behaviour.

In fact, since they do serve a similar purpose, we even support importing basic shell scripts into a profile using the `gpm profile import` command!

### Why isn't there a `man` page?

Because you're reading the documentation! GPM is a pretty simple tool but we believe that `man` pages are not the most helpful form of documentation (especially if you need to support Windows).

More importantly, thanks to the awesome `Spectre.Cli` project, you can always add `--help` to get full contextual information on how to use GPM.

### Now my Git config is borked and it's all your fault!

Well now, that seems harsh. Git Profile Manager relies on the underlying system Git to run configuration, and will only run the config items you add to the profile.

Even so, if things look to be a bit messed up, you can always just run `gpm deactivate <profile-name>` to unset the same config values. If you've activated multiple profiles, deactivate them in reverse order!