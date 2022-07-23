# Executing VoiceAttack Commands

VoicAttack’s plugin API allows you to run commands from your plugin:

```csharp
vaProxy.Command.Execute(
    string CommandPhrase,
    bool WaitForReturn,
    bool AsSubcommand,
    Action<Guid?> CompletedAction,
    string PassedText,
    srting PassedIntegers,
    string PassedDecimals,
    string PassedBooleans,
    string PassedDates);
```

None of those parameters are optional, and the arguments have to be passed as
semicolon delimited strings. With extra hassle around quoting `string`
arguments.

YAVAPF aims to make this API a little more comfortable to work with.

## Running a Command

```csharp
Plugin.Commands.Run(
    string command,
    bool logMissing = true,
    bool wait = false,
    bool subcommand = false,
    dynamic[]? parameters = null);
```

The main difference here is that all parameters apart from the command name are
now _optional_. You can also now pass arguments to the command as typed arrays:

```csharp
Plugin.Commands.Run(
    "example command",
    parameters: new dynamic[]
        {
            new string[] { "text value", "other text value" },
            new bool[] { true }
        });
```

The ability to pass a callback `Action<Guid?>` is not exposed. The only
information this `Action` can ever receive is the `Guid` of the command that has
been run; setting such a `Guid` in a profile though is a fairly involved
process, and exporting / importing the profile might not even preserve it.
Tl;dr: it is kind of useless right now.

## Running a Command with Prefixes

The `RunAll` method lets you run a command with multiple prefixes. This is
especially useful if your plugin accompanies a set of multiple VoiceAttack
profiles, [like my Elite Dangerous profiles and their correspondingp
plugins](https://alterNERDtive.github.io/VoiceAttack-profiles).

```csharp
Plugin.Commands.RunAll(new string[] { "EliteAttack", "RatAttack" }, "startup", …);
```

Prefixes are prepended to the command name with a dot. The line above would run
the `EliteAttack.startup` and `RatAttack.startup` commands.

## Running an Event

VoiceAttack does not actually have a concept of running commands on events. The
workaround is to have a naming convention for VoiceAttack “event” commands.

YAVAPF borrows the [`((command))` convention from
EDDI](https://github.com/EDCD/EDDI/wiki/VoiceAttack-Integration#running-commands-on-eddi-events).

```csharp
Plugin.Commands.TriggerEvent("some event", …);
```

This would run the `((some event))` command in VoiceAttack. Optional parameters
are the same as for [the `Run` method](commands.md#running-a-command).

## Running an Event with Prefixes

Similar to [`RunAll`](commands.md#running-a-command-with-prefixes),
`TriggerEventAll` exists.

```csharp
Plugin.Commands.TriggerEventAll(new string[] { "profile", "otherprofile" }, "event", …);
```

This would run the `((profile.event))` and `((otherprofile.event))` comands in
VoiceAttack.
