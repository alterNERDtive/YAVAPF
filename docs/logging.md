# Logging

YAVAPF allows logging to the VoiceAttack event log.

Logging to a log file is planned, but not implemented yet.

## Write a Log Line

To write a log message from plugin code, use the methods provided by the
`VoiceAttackLog` object availabe in the `Log` property of your plugin object.
There is one per log level.

```csharp
Plugin.Log.Error("Example error message.");
Plugin.Log.Debug("Just sent an error message.");
```

You can also log messages from a VoiceAttack command. Unlike a regular “Write to
Log” command action going through the plugin will enforce the correct format and
log level.

Your plugin will automatically provide the reserved plugin contexts
`log.<log level>` for each of the 5 log levels. Simply set a `~message` string
and call the appropriate plugin context. This should be equivalent to the code
above:

```
Set text [~message] to 'Example error message.'
Execute external plugin, 'Your Plugin v0.0.1' using context 'log.error' and wait for return
Set text [~message] to 'Just sent an error message.'
Execute external plugin, 'Your Plugin v0.0.1' using context 'log.debug' and wait for return
```

## Log Level

A message will be colour coded with the corresponding colour. If the log level
assigned to a message is below the current log level, it will not be displayed.

E.g. an `INFO` message will not be displayed by default since the default log
level is `NOTICE`. A `DEBUG` message will only ever be displayed if the current
log level is `DEBUG`.

| Log Level	| Log Colour	| Recommended Use
|-----------|---------------|----------------------------
| ERROR		| 🟥 red			| unrecoverable error
| WARN		| 🟨 yellow		| recoverable error, warning
| NOTICE	| 🟩 green		| noteworthy information
| INFO		| 🟦 blue		| miscellaneous information
| DEBUG		| ⬜ gray		| debugging

## Setting the Current Log Level

You can set the current log level by either setting the `LogLevel` property or
by invoking `SetLogLevel(string)` of your plugin’s `Log` property.

The latter is mostly useful when dealing with input from a VoiceAttack command.
Its parameter is not case sensitive.

```csharp
Plugin.Log.LogLevel = LogLevel.WARN;
Plugin.Log.SetLogLevel("info");
```

You can also set the log level from a VoiceAttack command directly by simply
setting `<plugin name>.loglevel#` to the desired log level.

```
Set text [Your Plugin.loglevel#] to 'debug'
```

The variable changed event for this specific variable name will be handled by
YAVAPF internally. You can of course still define your own handlers in addition
to it.
