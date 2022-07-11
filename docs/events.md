# Handling VoiceAttack Events

In order to handle VoiceAttack’s `Init`, `Exit` and `StopCommand` events, you
will have to define corresponding event handlers. [The `Invoke` event is handled
separately](contexts.md).

Generally speaking, event handlers in YAVAPF are `public static` methods of your
plugin class that must have certain attributes associated to them and must have
the correct method signature.

An event can have as many handlers as you require. Do note that a specific order
of execution cannot be guaranteed.

## Init

`Init` handlers are invoked when VoiceAttack inintializes your plugin. That
happens exactly once at application startup. Use these to setup your plugin for
use.

`Init` handlers must accept a single `VoiceAttackInitProxyClass` parameter and
must have an `InitAttribute`. `InitAttribute` does not have any properties.

```csharp
[Init]
public static void MyInitHandler(VoiceAttackInitProxyClass vaProxy)
{
	[…]
}
```

## Exit

`Exit` handlers are invoked when VoiceAttack closes. That happens exactly once
at application shutdown. Use these to gracefully tear down anything your plugin
has to tear down.

`Exit` handlers must accept a single `VoiceAttackProxyClass` parameter and must
have an `ExitAttribute`. `ExitAttribute` does not have any properties.

```csharp
[Exit]
public static void MyExitHandler(VoiceAttackProxyClass vaProxy)
{
	[…]
}
```

## StopCommand

`StopCommand` handlers are invoked whenever VoiceAttack stops all commands. That
happens e.g. when a “Stop all commands” command action is executed or when the
“Stop Commands” button on the main interface is pressed. If your plugin has to
respond to that, use these.

`StopCommand` handlers must have no parameters and must have a
`StopCommandAttribute`. `StopCommandAttribute` does not have any properties.

```csharp
[StopCommand]
public static void MyStopCommandHandler()
{
	[…]
}
```
