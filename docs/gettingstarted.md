# Getting Started

First off, you can see [the Example plugin
project](https://github.com/alterNERDtive/YAVAPF/tree/release/ExamplePlugin) on
Github for reference.

Second off, this documentation assumes that you have at least cross read the
section about plugins [in the VoiceAttack
manual](https://voiceattack.com/VoiceAttackHelp.pdf). If any terminology is new
to you, it is probably introduced there. Unlike said manual though this will
provide step by step instructions to get your plugin set up.

## Creating a Visual Studio Project

I am going to assume for this part of the documentation that you are using
Visual Studio 2022 or later (_not_ Visual Studio Code!) as your development
environment. [The Community Edition is free for unlimited time personal
use](https://visualstudio.microsoft.com/vs/compare/).

VoiceAttack is a .Net Framework 4.8 application. Plugins targeting .Net 5+ or
.Net Core will not work. I still recommend creating a .Net project instead of a
.Net Framework project, then changing the “Target Framework” to .Net Framework
4.8. This allows you to use the full `dotnet` tool chain, which makes e.g. using
Github Actions to build / release your project much less painful. Trust me, I’ve
done it both ways.

So, create a new “Class Library” project, then use a text editor to change the
“TargetFrameworks” property to `net48`. While you’re there you might also want
to change the “LanguageVerison” to `10`. Most new features are backwards
compatible with .Net Framework. The compiler will assist you with errors for
those that are not.

## Adding YAVAPF as a Dependency

This one is the simple part, just install `alterNERDtive.YAVAPF` through NuGet.
Done.

Alternatively you can add it manually by cloning
`github.com/alterNERDtive/YAVAPF.git` as a git submodule and referencing
`VoiceAttack-Framework\VoiceAttack-Framework.csproj` as a project reference.

But seriously, use NuGet. I haven’t taught myself how to release NuGet packages
just for you to ignore it!

## Adding VoiceAttack as a Dependency

This is a little more involved. In order to use the actual proxy classes from
VoiceAttack instead of the “official” crutch of `dynamic` types, you will need
to add an assembly reference to `VoiceAttack.exe`.

Right click → “Add” → “Assembly Reference…” → “Browse” → browse to the
VoiceAttack installation folder → select `VoiceAttack.exe` → hit “Add” → make
sure it is ticked in the list → hit “OK”.

Now, we want to _reference_ `VoiceAttack.exe`, but we don’t want to _include_ it
when compiling the plugin. So select “VoiceAttack” in “Dependencies” →
“Assemblies” and make sure that both “Copy Local” and “Embed Interop Types” are
set to “No”.

Distributing `VoiceAttack.exe` with your plugin would technically be a copyright
violation. _Do_ make sure to take the steps outlined in the last paragraph to
prevent accidentally doing that! Using it as a reference assembly is generally
OK and I have received confirmation from Gary, the author of VoiceAttack.

## Setting Up Debugging Through VoiceAttack

In order to be able to run VoiceAttack when debugging and actually debug your
plugin, you will need to open “Debug” → “<your project\> Debug Properties”.

There you will need to “Create a new profile” → “Executable”. Set the path to
your VoiceAttack executable and any command line options you might prefer.
Personally I like to set a custom `-datadir` in order to not mess with my
regular profile database accidentally.

The example plugin project has a `Properties\launchSettings.sample.json` file
that you can copy to `Properties\launchSettings.json` and edit accordingly to
accomplish the same thing.

The last thing you’ll need to do is make your plugin available to VoiceAttack
in a place where it can find it. I have requested an equivalent `-appdir`
parameter, but as long as that is not available you will need to have your
plugin present inside the regular `Apps` folder of VoiceAttack. I recommend
creating a directory junction (`mklink /j`, or `New-Item -ItemType Junction` in
PowerShell) between an `Apps` subfolder and your project’s debug output
directory (usually `<project name>\bin\Debug\net48` inside your solution
folder).

## Building Through Github Actions

If you, like me, want to automate building/testing/releasing through [Github
Actions](https://docs.github.com/en/actions), you’ll need to have VoiceAttack
available while building on the worker. Obviously that will only work on a
Windows worker.

I have created the
[`alterNERDtive/setup-voiceattack-action`](https://github.com/alterNERDtive/setup-voiceattack-action)
to facilitate that. Usage example:

```yaml
- name: Install VoiceAttack
  uses: alterNERDtive/setup-voiceattack-action
  with:
    version: "1.10"
```

Make sure that the path to VoiceAttack on your machine (which is the path
referenced in the project file) matches the path where you install VoiceAttack
on the worker! Alternatively, if you have installed VoiceAttack in a custom
folder locally, you can create a symlink (`mklink`, or
`New-Item -ItemType SymbolicLink` in PowerShell) to your `VoiceAttack.exe`
location at `C:\Program Files\VoiceAttack\VoiceAttack.exe` and include that as
the assembly reference.

## Creating a Minimum Viable Plugin

A valid VoiceAttack plugin must implement a selection of public, static methods:

* `VA_DisplayName()`: Must return the name of the plugin.
* `VA_DisplayInfo()`: Must return the description of the plugin.
* `VA_Id()`: Must return the GUID of the plugin.
* `VA_Init1(dynamic)`: Is executed when the plugin is loaded into VoiceAttack.
* `VA_Invoke1(dynamic)`: Is executed whenever a plugin context is run from a
  command.
* `VA_Exit1(dynamic)`: Is executed when VoiceAttack shuts down.
* `VA_StopCommand()`: Is executed when VoiceAttack stops all commands, e.g.
  through the command action or main interface button.

When using YAVAPF these methods are to be passed straight to the corresponding
methods of a `VoiceAttackPlugin` object that handles most things for you. It has
a few required properties:

* `Name`: The name of the plugin.
* `Version`: The version of the plugin.
* `Info`: The description of the plugin.
* `Guid`: The GUID of the plugin.

All of those are `string`s for ease of use, though the `Guid` obviously has to
be a valid string representation of a GUID. You can generate one using “Tools” →
“Create GUID”. Make sure to select “Registry Format”.

For a YAVAPF plugin you will have to derive your plugin class from
`alterNERDtive.Yavapf.VoiceAttackPlugin`. Since VoiceAttack’s plugin API relies
entirely on static methods, you’ll need to instantiate your plugin object in its
static constructor and hold it in a static variable for future reference (no pun
intended).

So a minimum viable plugin using YAVAPF looks kind of like this:

```csharp
using System;

using alterNERDtive.Yavapf;

namespace YourNamespace
{
    public class YourPlugin : VoiceAttackPlugin
    {
        private static readonly YourPlugin Plugin;

        static YourPlugin()
        {
            Plugin = new ()
            {
                Name = "Your Plugin",
                Version = "0.0.1",
                Info = "This is a description",
                Guid = "{5E93F293-B2CB-4B3F-AFC5-AE500A7EEBA9}",
            };
        }

        public static string VA_DisplayName() => Plugin.VaDisplayName();

        public static string VA_DisplayInfo() => Plugin.VaDisplayInfo();

        public static Guid VA_Id() => Plugin.VaId();

        public static void VA_Init1(dynamic vaProxy) => Plugin.VaInit1(vaProxy);

        public static void VA_Invoke1(dynamic vaProxy) => Plugin.VaInvoke1(vaProxy);

        public static void VA_Exit1(dynamic vaProxy) => Plugin.VaExit1(vaProxy);

        public static void VA_StopCommand() => Plugin.VaStopCommand();
    }
}
```

That’s it! Technically you’re done. Hit the debug button, and VoiceAttack should
find your plugin on startup, report loading it in the event log, and list it
under “Options” → “General” → “Plugin Manager”.

Of course you are only just getting started if you want your plugin to actually
_do_ something!
