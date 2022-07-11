# Defining Plugin Contexts

Plugin contexts are defined similarly to [event handlers](events.md).

They are `public static` methods of your plugin class that must have a
`ContextAttribute` and must accept a `VoiceAttackInvokeProxyClass` parameter.

`ContextAttribute` has a single property `Name`. `Name` can either be the name
of a plugin context or a regular expression defining all plugin contexts it
should be associated with. `Name` is set through an optional parameter of the
attribute constructor; if it is omitted, the method will be executed for any
plugin context.

A method can have multiple `ContextAttribute`s. It will be executed if any of
them matches the context of a plugin invocation. That also means that you can
have several methods that handle the same plugin context; as with [event
handlers](events.md), a specific order of execution cannot be guaranteed.

If your method handles multiple contexts, the context it was invoked with can be
found in the `Context` property of its `VoiceAttackInvokeProxyClass` parameter.

**Note**: The `log.*` context is reserved [for
logging](logging.md#from-a-voiceattack-command) and cannot be used for your
plugin.

## Named Plugin Contexts

For singular context names, add a `ContextAttribute` for each name. Context
names are to be lower case by convention.

This should be the default way to handle plugin contexts, and multiple contexts
handled by the same method should be alternate names for the same functionality.
Separate functionality, separate handler method(s).

```csharp
[Context("test")]
[Context("test context")]
[Context("alternate context name")]
public static void TestContext(VoiceAttackInvokeProxyClass vaProxy)
{
	[…]
}
```

## Regular Expression Plugin Contexts

For contexts defined by regular expressions, the `Name` property must start with
a `^` to be recognized as a regular expression. Incidentally that means you have
to define your regular expression to match from the beginning of the context
string.

The main use case for regular expression contexts is grouping contexts that
logically belong together or behave in very similar ways. For example you could
have a single `^edsm\..*` context in an Elite Dangerous related plugin that
handles anything related to querying [EDSM](https://edsm.net).

As with [catchall contexts](#catchall-plugin-contexts), you should probably have
some kind of way to differentiate between contexts. For any contexts that match
the regular expression(s) but are not valid contexts for your plugin, `throw` an
`ArgumentException` with “context” as the parameter name. The exception message
can be anything, it will not be used.

Oh, and of course you can combine named and regular expression contexts. This
example features some different regular expressions and corresponding
conditionals:

```csharp
[Context(@"^foo.*")]
[Context(@"^.*bar.*")]
[Context(@"^.*baz")]
[Context("some name")]
public static void RegexContext(VoiceAttackInvokeProxyClass vaProxy)
{
	string context = vaProxy.Context;
	if (context.StartsWith("foo")) {
		[…]
	}
	else if (context.Contains("bar")) {
		[…]
	}
	else if (context.EndsWith("baz")) {
		[…]
	}
	else if (context == "some name")) {
		[…]
	}
	else {
		throw new ArgumentException("", "context");
	}
}
```

This example is more focused and closer to how regular expression contexts are
intended to be used in practice:

```csharp
[Context(@"^edsm\..*")]
public static void EdsmContext(VoiceAttackInvokeProxyClass vaProxy)
{
	switch(vaProxy.Context)
	{
		case "edsm.findsystem":
			[…]
			break;
		case "edsm.findcommander":
			[…]
			break;
		case "edsm.trafficreport":
			[…]
			break;
		default:
			throw new ArgumentException("", "context");;
	}
}
```

## “Catchall” Plugin Contexts

To have a method invoked on any plugin invocation regardless of context, add a
`ContextAttribute` and omit the `Name`.

**This is not recommended** and has similar issues to using the bare VoiceAttack
plugin API. It is mostly provided for backwards compatibility; you can easily
convert your old `VA_Invoke1(dynamic)` method to a catchall plugin context and
then modify from there.

As with [regular expression contexts](#regular-expression-plugin-contexts), you
should probably have some kind of way to differentiate between contexts. For any
contexts that are not valid contexts for your plugin, `throw` an
`ArgumentException` with “context” as the parameter name. The exception message,
again, doesn’t matter.

```csharp
[Context]
public static void CatchallContext(VoiceAttackInvokeProxyClass vaProxy)
{
	switch (vaProxy.Context)
	{
		case "some context":
			[…]
			break;
		case "some other context":
			[…]
			break;
		default:
			throw new ArgumentException("", "context");;
	}
}
```

## Context Parameters

VoiceAttack plugin contexts by design do not have any parameters. If you need
data passed from a VoiceAttack command to the plugin when a context is invoked,
you will have to set a variable in your VoiceAttack command and then retrieve
said variable from the context handler method.

In general it is recommended to provide context parameters as command scoped
variables (`~` prefix) in order not to interfere with other commands / plugin
invocations and their data.

This example accesses the `~test` text variable from plugin code:

```csharp
string? testParameter = vaProxy.Get<string>("~test");
```

In case a parameter is missing that is _required_ for your context `throw` an
`ArgumentNullException` with the variable name as the parameter name:

```csharp
string testParameter = vaProxy.Get<string>("~test") ?? throw new ArgumentNullException("~test");
```

**Note**: You should always use the `Get<T>(string)` and `Set<T>(string)`
extension methods of the `VoiceAttackInvokeProxyClass` object when accessing
command scoped variables. They are only available in the proxy object passed to
the corresponding context handler; the cached proxy object of your plugin object
might have changed already, leading to race conditions.

[More about variables](variables.md).
