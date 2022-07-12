# Using VoiceAttack Variables

VoiceAttack allows you to set a plethora of variables. Each variable has

* a name
* a type
* a scope

Variable names are unique to each variable type. E.g. you can have _both_ a text
variable `test` _and_ a boolean variable `test`.

In addition to that variable names are also unique to each _scope_; or,
technically speaking, the scope is part of the variable name. E.g. a text
variable `~test` is different from a text variable `>test`, and both are
different from a text variable `test`.

## Variable Types

| VoiceAttack type		| .Net type | Note 
|-----------------------|-----------|-------------------------------------
| Text					| string	|
| True/False (Boolean)	| bool		|
| Integer				| int		|
| Decimal				| decimal	|
| Date/Time				| DateTime	|
| Small Integer			| short		| deprecated; not supported by YAVAPF

Technical note: VoiceAttack internally holds a `Dictionary<string, T>` for each
variable type. That is why, unlike regular variables in a .Net scope, the names
are unique to their type.

## Variable Scopes

| Prefix	| Scope					| Accessibility
|-----------|-----------------------|-------------------------------------------------
| none		| global				| everywhere
| `>>`		| profile, persistent	| same profile, preserved across profile switches
| `>`		| profile				| same profile, reset on profile switch
| `~~`		| command + subcommands	| this command invocation and its subcommands
| `~`		| command only			| this command invocation

There is no scope that retains variable values across VoiceAttack restarts. You
can save variable values to / load them from the current profile using a “Set a
<Type\> Value” command action and ticking the corresponding box, but there is
currently no way to do it from a plugin. Feature request pending.

Restricting variable scope as far as possible is recommended. For communication
between commands and their plugin invocations scope should almost always be
command only (`~`).

For global commands used by your plugin having some unique prefix is a sensible
idea. For example, YAVAPF automatically sets the text variable
`<plugin name>.version` to the current version of your plugin.

## Default Variables

By default, YAVAPF automatically sets the following variables for your plugin:

| Variable						| Type		| Description
|-------------------------------|-----------|--------------------------------------------
| `<plugin name>.version`		| string	| The current version of your plugin.
| `<plugin name>.initialized`	| bool		| The plugin has been initialized correctly.

## Using Proxy Methods vs. Using Plugin Methods

YAVAPF extends the `VoiceAttackInitProxyClass` and `VoiceAttackInvokeProxyClass`
classes with new generic methods for getting and setting variables. On top of
that `VoiceAttackPlugin` objects provide the same methods for ease of use and
when no proxy object is readily available to the current code path.

There is one **caveat** here: plugin objects cache a new proxy object every time
a plugin context is invoked. If you need to access command scoped variables (`~`
or `~~`), you should use the `VoiceAttackInvokeProxyClass` object directly.
Otherwise you might run into race conditions.

Correct:

```csharp
[Context("test")]
private static void TestContext(VoiceAttackInvokeProxyClass vaProxy)
{
    string test = vaProxy.Get<string>("~test");
}
```

Incorrect, might lead to race condition:

```csharp
[Context("test")]
private static void TestContext(VoiceAttackInvokeProxyClass vaProxy)
{
    string test = Plugin.Get<string>("~test");
}
```

This will log a warning to the VoiceAttack event log.

You can, if you are sure that you will _not_ run into a race condition, suppress
said warning:

```csharp
[Context("test")]
private static void TestContext(VoiceAttackInvokeProxyClass vaProxy)
{
    string test = Plugin.Get<string>("~test", suppressWarning: true);
}
```

## Getting Variable Values

To get the value of a variable, invoke the `Get<T>(string name)` method of your
plugin where `T` is the type of the variable and `name` is its name including
its scope:

```csharp
string? foo = Plugin.Get<string>("foo");
bool bar = vaProxy.Get<bool>("~bar") ?? false;
```

Remember that variable values will be returned as `null` (“Not Set” in
VoiceAttack terminology) if they are currently not holding a value.

## Setting Variable Values

To set the value of a variable, invoke the `Set<T>(string name, T value)` method
of your plugin where `T` is the type of the variable, `name` is its name
including its scope and `value` is the desired new value:

```csharp
Plugin.Set<DateTime>("current", DateTime.Now);
Plugin.Set<int>(">>deaths", (Plugin.Get<int>(">>deaths") ?? 0) + 1);
```

## Clearing Variable Values

To clear a variable, invoke the `UnSet<T>(string name)` method of your plugin
where `T` is the type of the variable and `name` is its name including its
scope:

```csharp
Plugin.UnSet<decimal>("π");
```

Or `Set<T>(string, T)` it to `null`:

```csharp
Plugin.Set<string>(">fizzbang", null);
```

## Subscribing to “Variable Changed” Events

VoiceAttack allows triggering plugins when a variable value changes. Handlers
must have an Attribute corresponding to the variable type (`BoolAttribute`,
`DateTimeAttribute`, `DecimalAttribute`, `IntAttribute`, `StringAttribute`) and
accept the following parameters:

* `string name`: the name of the variable that has changed
* `T? from`: the old value of the variable
* `T? to`: the new value of the variable

where `T` is the type of the variable. Remember that at any point either  `to`
or `from` might be `null`.

**Note**: In order for a variable to trigger variable changed events, the
variable name **must** end with a number sign (`#`)! This is a limitation
enforced by VoiceAttack. The purpose of this constraint is to not constantly
invoke plugins whenever any variable changes.

```csharp
[Bool("isDay#")]
public static void DayChanged(string name, bool? from, bool? to)
{
    Plugin.Log.Notice($"It is now {(to ?? false ? "day" : "night")}.");
}
```

This constraint also applies to catchall handlers. Even though the following
method accepts any variable name, it will still only be invoked if the name of a
changed variable ends with a `#`. E.g. changing the text variable `foo#` will
invoke it while changing the text variable `foo` will not.

```csharp
[String]
public static void StringChanged(string name, string? from, string? to)
{
    Plugin.Log.Notice($"Text variable '{name}' changed from '{from ?? "Not Set"}' to '{to ?? "Not Set"}'.");
}
```

[More on logging](logging.md).

Do be aware that changing the value of a variable from within its handler will
trigger another variable changed event and run the handler again. It is very
much possible to create an infinite loop.

Attribute names work in the same way as [context names](contexts.md). You can
have handlers for singular variable names, regular expressions, and catchall
handlers.

The text variable `<plugin name>.loglevel#` is handled by YAVAPF internally to
set the current log level. You can still add additional handlers for this
variable.
