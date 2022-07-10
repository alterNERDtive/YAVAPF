// <copyright file="VoiceAttackPlugin.cs" company="alterNERDtive">
// Copyright 2022 alterNERDtive.
//
// This file is part of YAVAPF.
//
// YAVAPF is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// YAVAPF is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with YAVAPF.  If not, see &lt;https://www.gnu.org/licenses/&gt;.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using VoiceAttack;

namespace alterNERDtive.Yavapf
{
    /// <summary>
    /// Framework class for implementing a VoiceAttack plugin.
    /// </summary>
    public class VoiceAttackPlugin
    {
        private VoiceAttackLog? log;
        private VoiceAttackInitProxyClass? vaProxy;

        /// <summary>
        /// Invoked when VoiceAttack initializes the plugin.
        /// </summary>
        protected event Action<VoiceAttackInitProxyClass>? InitActions;

        /// <summary>
        /// Invoked when VoiceAttack closes.
        /// </summary>
        protected event Action<VoiceAttackProxyClass>? ExitActions;

        /// <summary>
        /// Invoked when VoiceAttack stops all commands.
        /// </summary>
        protected event Action? StopActions;

        /// <summary>
        /// Gets or sets the Actions to be run when the plugin is invoked from a
        /// VoiceAttack command. Only Actions with a matching “Context”
        /// attribute will be invoked.
        /// </summary>
        protected HandlerList<Action<VoiceAttackInvokeProxyClass>> Contexts { get; set; } = new ();

        /// <summary>
        /// Gets or sets the Actions to be run when a <see cref="bool"/>
        /// variable changed.
        /// </summary>
        protected HandlerList<Action<string, bool?, bool?>> BoolChangedHandlers { get; set; } = new ();

        /// <summary>
        /// Gets or sets the Actions to be run when a <see cref="DateTime"/>
        /// variable changed.
        /// </summary>
        protected HandlerList<Action<string, DateTime?, DateTime?>> DateTimeChangedHandlers { get; set; } = new ();

        /// <summary>
        /// Gets or sets the Actions to be run when a <see cref="decimal"/>
        /// variable changed.
        /// </summary>
        protected HandlerList<Action<string, decimal?, decimal?>> DecimalChangedHandlers { get; set; } = new ();

        /// <summary>
        /// Gets or sets the Actions to be run when a <see cref="int"/>
        /// variable changed.
        /// </summary>
        protected HandlerList<Action<string, int?, int?>> IntChangedHandlers { get; set; } = new ();

        /// <summary>
        /// Gets or sets the Actions to be run when a <see cref="string"/>
        /// variable changed.
        /// </summary>
        protected HandlerList<Action<string, string?, string?>> StringChangedHandlers { get; set; } = new ();

        /// <summary>
        /// Gets the currently stored VoiceAttackInitProxyClass object which is
        /// used to interface with VoiceAttack.
        ///
        /// You will usually want to use the provided methods and Properties
        /// instead.
        /// </summary>
        protected VoiceAttackInitProxyClass Proxy
        {
            get => this.vaProxy!;
        }

        /// <summary>
        /// Gets or sets the name of the plugin.
        /// </summary>
        protected string? Name { get; set; }

        /// <summary>
        /// Gets or sets the current version of the plugin.
        /// </summary>
        protected string? Version { get; set; }

        /// <summary>
        /// Gets or sets the description of the plugin.
        /// </summary>
        protected string? Info { get; set; }

        /// <summary>
        /// Gets or sets the GUID of the plugin.
        /// </summary>
        protected string? Guid { get; set; }

        /// <summary>
        /// Gets the <see cref="VoiceAttackLog"/> instance the plugin uses to
        /// log to the VoiceAttack event log.
        ///
        /// You can use this to log your own messages.
        /// </summary>
        protected VoiceAttackLog Log
        {
            get => this.log ??= new VoiceAttackLog(this.Proxy, this.Name!);
        }

        /// <summary>
        /// Gets the value of a variable from VoiceAttack.
        ///
        /// Valid varible types are <see cref="bool"/>, <see cref="DateTime"/>,
        /// <see cref="decimal"/>, <see cref="int"/> and <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The value of the variable. Can be null.</returns>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        protected T? Get<T>(string name)
        {
            dynamic? ret = typeof(T) switch
            {
                Type boolType when boolType == typeof(bool) => this.Proxy.GetBoolean(name),
                Type dateType when dateType == typeof(DateTime) => this.Proxy.GetDate(name),
                Type decType when decType == typeof(decimal) => this.Proxy.GetDecimal(name),
                Type intType when intType == typeof(int) => this.Proxy.GetInt(name),
                Type stringType when stringType == typeof(string) => this.Proxy.GetText(name),
                _ => throw new InvalidDataException($"Cannot get variable '{name}': invalid type '{typeof(T).Name}'."),
            };
            return ret;
        }

        /// <summary>
        /// Sets a variable for use in VoiceAttack.
        ///
        /// Valid varible types are <see cref="bool"/>, <see cref="DateTime"/>,
        /// <see cref="decimal"/>, <see cref="int"/> and <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable. Can not be null.</param>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        protected void Set<T>(string name, T? value)
        {
            if (value == null)
            {
                this.Unset<T>(name);
            }
            else
            {
                switch (value)
                {
                    case bool b:
                        this.Proxy.SetBoolean(name, b);
                        break;
                    case DateTime d:
                        this.Proxy.SetDate(name, d);
                        break;
                    case decimal d:
                        this.Proxy.SetDecimal(name, d);
                        break;
                    case int i:
                        this.Proxy.SetInt(name, i);
                        break;
                    case string s:
                        this.Proxy.SetText(name, s);
                        break;
                    default:
                        throw new InvalidDataException($"Cannot set variable '{name}': invalid type '{typeof(T).Name}'.");
                }
            }
        }

        /// <summary>
        /// Unsets a variable for use in VoiceAttack (= sets it to null).
        ///
        /// Valid varible types are <see cref="bool"/>, <see cref="DateTime"/>,
        /// <see cref="decimal"/>, <see cref="int"/> and <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        protected void Unset<T>(string name)
        {
            switch (typeof(T))
            {
                case Type boolType when boolType == typeof(bool):
                    this.Proxy.SetBoolean(name, null);
                    break;
                case Type dateType when dateType == typeof(DateTime):
                    this.Proxy.SetDate(name, null);
                    break;
                case Type decType when decType == typeof(decimal):
                    this.Proxy.SetDecimal(name, null);
                    break;
                case Type intType when intType == typeof(int):
                    this.Proxy.SetInt(name, null);
                    break;
                case Type stringType when stringType == typeof(string):
                    this.Proxy.SetText(name, null);
                    break;
                default:
                    throw new InvalidDataException($"Cannot set variable '{name}': invalid type '{typeof(T).Name}'.");
            }
        }

        /// <summary>
        /// The plugin’s display name, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The display name.</returns>
        protected string VaDisplayName() => $"{this.Name} v{this.Version}";

        /// <summary>
        /// The plugin’s description, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The description.</returns>
        protected string VaDisplayInfo() => this.Info!;

        /// <summary>
        /// The plugin’s GUID, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The GUID.</returns>
        protected Guid VaId() => new (this.Guid);

        /// <summary>
        /// The Init method, as required by the VoiceAttack plugin API.
        /// Runs when the plugin is initially loaded.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        protected void VaInit1(VoiceAttackInitProxyClass vaProxy)
        {
            this.vaProxy = vaProxy;

            this.Set<string>($"{this.Name}.version", this.Version);
            this.Log.Debug($"Initializing v{this.Version} …");

            this.vaProxy.BooleanVariableChanged += this.BooleanVariableChanged;
            this.vaProxy.DateVariableChanged += this.DateVariableChanged;
            this.vaProxy.DecimalVariableChanged += this.DecimalVariableChanged;
            this.vaProxy.IntegerVariableChanged += this.IntegerVariableChanged;
            this.vaProxy.TextVariableChanged += this.TextVariableChanged;

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<InitAttribute>().Any()).ToList().ForEach(
                m => this.InitActions += (Action<VoiceAttackInitProxyClass>)m.CreateDelegate(typeof(Action<VoiceAttackInitProxyClass>)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<ExitAttribute>().Any()).ToList().ForEach(
                m => this.ExitActions += (Action<VoiceAttackProxyClass>)m.CreateDelegate(typeof(Action<VoiceAttackProxyClass>)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<StopCommandAttribute>().Any()).ToList().ForEach(
                m => this.StopActions += (Action)m.CreateDelegate(typeof(Action)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<ContextAttribute>().Any()).ToList().ForEach(
                m => this.Contexts += (Action<VoiceAttackInvokeProxyClass>)m.CreateDelegate(typeof(Action<VoiceAttackInvokeProxyClass>)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<BoolAttribute>().Any()).ToList().ForEach(
                m => this.BoolChangedHandlers += (Action<string, bool?, bool?>)m.CreateDelegate(typeof(Action<string, bool?, bool?>)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<DateTimeAttribute>().Any()).ToList().ForEach(
                m => this.DateTimeChangedHandlers += (Action<string, DateTime?, DateTime?>)m.CreateDelegate(typeof(Action<string, DateTime?, DateTime?>)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<DecimalAttribute>().Any()).ToList().ForEach(
                m => this.DecimalChangedHandlers += (Action<string, decimal?, decimal?>)m.CreateDelegate(typeof(Action<string, decimal?, decimal?>)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<IntAttribute>().Any()).ToList().ForEach(
                m => this.IntChangedHandlers += (Action<string, int?, int?>)m.CreateDelegate(typeof(Action<string, int?, int?>)));

            this.GetType().GetMethods().Where(m => m.GetCustomAttributes<StringAttribute>().Any()).ToList().ForEach(
                m => this.StringChangedHandlers += (Action<string, string?, string?>)m.CreateDelegate(typeof(Action<string, string?, string?>)));

            this.Log.Debug("Running Init handlers …");
            this.InitActions?.Invoke(vaProxy);
            this.Log.Debug("Finished running Init handlers.");

            this.Set<bool>($"{this.Name}.initialized", true);
            this.Log.Debug("Initialized.");
        }

        /// <summary>
        /// The Invoke method, as required by the VoiceAttack plugin API.
        /// Runs whenever a plugin context is invoked.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        protected void VaInvoke1(VoiceAttackInvokeProxyClass vaProxy)
        {
            this.vaProxy = vaProxy;

            string context = vaProxy.Context.ToLower();

            if (context.StartsWith("log."))
            {
                try
                {
                    string message = this.Get<string>("~message") ?? throw new ArgumentNullException("~message");
                    switch (context)
                    {
                        case "log.error":
                            this.Log.Error(message);
                            break;
                        case "log.warn":
                            this.Log.Warn(message);
                            break;
                        case "log.notice":
                            this.Log.Notice(message);
                            break;
                        case "log.info":
                            this.Log.Info(message);
                            break;
                        case "log.debug":
                            this.Log.Debug(message);
                            break;
                        default:
                            throw new ArgumentException("invalid context", "context");
                    }
                }
                catch (ArgumentNullException e)
                {
                    this.Log.Error($"Missing parameter '{e.ParamName}' for context '{context}'");
                }
                catch (ArgumentException e) when (e.ParamName == "context")
                {
                    this.Log.Error($"Invalid plugin context '{vaProxy.Context}'.");
                }
                catch (Exception e)
                {
                    this.Log.Error($"Unhandled exception while executing plugin context '{context}': {e.Message}");
                }
            }
            else
            {
                List<Action<VoiceAttackInvokeProxyClass>> actions = this.Contexts.Where(
                    action => action.Method.GetCustomAttributes<ContextAttribute>().Where(
                        attr => attr.Name == context ||
                            (attr.Name.StartsWith("^") && Regex.Match(context, attr.Name).Success))
                            .Any()).ToList();

                if (actions.Any())
                {
                    foreach (Action<VoiceAttackInvokeProxyClass> action in actions)
                    {
                        try
                        {
                            action.Invoke(vaProxy);
                        }
                        catch (ArgumentNullException e)
                        {
                            this.Log.Error($"Missing parameter '{e.ParamName}' for context '{context}'");
                        }
                        catch (ArgumentException e) when (e.ParamName == "context")
                        {
                            this.Log.Error($"Invalid plugin context '{vaProxy.Context}'.");
                        }
                        catch (Exception e)
                        {
                            this.Log.Error($"Unhandled exception while executing plugin context '{context}': {e.Message}");
                        }
                    }
                }
                else
                {
                    this.Log.Error($"Invalid plugin context '{vaProxy.Context}'.");
                }
            }
        }

        /// <summary>
        /// The Exit method, as required by the VoiceAttack plugin API.
        /// Runs when VoiceAttack is shut down.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        protected void VaExit1(VoiceAttackProxyClass vaProxy)
        {
            this.ExitActions?.Invoke(vaProxy);
        }

        /// <summary>
        /// The StopCommand method, as required by the VoiceAttack plugin API.
        /// Runs whenever all commands are stopped using the “Stop All Commands”
        /// button or action.
        /// </summary>
        protected void VaStopCommand()
        {
            this.StopActions?.Invoke();
        }

        /// <summary>
        /// Invoked when a <see cref="bool"/> variable changed.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="from">The old value of the variable.</param>
        /// <param name="to">The new value of the variable.</param>
        /// <param name="internalID">The internal GUID of the variable.</param>
        private void BooleanVariableChanged(string name, bool? from, bool? to, Guid? internalID = null)
        {
            foreach (Action<string, bool?, bool?> action in this.BoolChangedHandlers.Where(
                action => action.Method.GetCustomAttributes<BoolAttribute>().Where(
                    attr => attr.Name == name ||
                    (attr.Name.StartsWith("^") && Regex.Match(name, attr.Name).Success))
                .Any()).ToList())
            {
                try
                {
                    action.Invoke(name, from, to);
                }
                catch (Exception e)
                {
                    this.Log.Error($"Unhandled exception while handling changed bool variable '{name}': {e.Message}");
                }
            }
        }

        /// <summary>
        /// Invoked when a <see cref="DateTime"/> variable changed.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="from">The old value of the variable.</param>
        /// <param name="to">The new value of the variable.</param>
        /// <param name="internalID">The internal GUID of the variable.</param>
        private void DateVariableChanged(string name, DateTime? from, DateTime? to, Guid? internalID = null)
        {
            foreach (Action<string, DateTime?, DateTime?> action in this.DateTimeChangedHandlers.Where(
                action => action.Method.GetCustomAttributes<DateTimeAttribute>().Where(
                    attr => attr.Name == name ||
                    (attr.Name.StartsWith("^") && Regex.Match(name, attr.Name).Success))
                .Any()).ToList())
            {
                try
                {
                    action.Invoke(name, from, to);
                }
                catch (Exception e)
                {
                    this.Log.Error($"Unhandled exception while handling changed DateTime variable '{name}': {e.Message}");
                }
            }
        }

        /// <summary>
        /// Invoked when a <see cref="decimal"/> variable changed.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="from">The old value of the variable.</param>
        /// <param name="to">The new value of the variable.</param>
        /// <param name="internalID">The internal GUID of the variable.</param>
        private void DecimalVariableChanged(string name, decimal? from, decimal? to, Guid? internalID = null)
        {
            foreach (Action<string, decimal?, decimal?> action in this.DecimalChangedHandlers.Where(
                action => action.Method.GetCustomAttributes<DecimalAttribute>().Where(
                    attr => attr.Name == name ||
                    (attr.Name.StartsWith("^") && Regex.Match(name, attr.Name).Success))
                .Any()).ToList())
            {
                try
                {
                    action.Invoke(name, from, to);
                }
                catch (Exception e)
                {
                    this.Log.Error($"Unhandled exception while handling changed decimal variable '{name}': {e.Message}");
                }
            }
        }

        /// <summary>
        /// Invoked when a <see cref="int"/> variable changed.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="from">The old value of the variable.</param>
        /// <param name="to">The new value of the variable.</param>
        /// <param name="internalID">The internal GUID of the variable.</param>
        private void IntegerVariableChanged(string name, int? from, int? to, Guid? internalID = null)
        {
            foreach (Action<string, int?, int?> action in this.IntChangedHandlers.Where(
                action => action.Method.GetCustomAttributes<IntAttribute>().Where(
                    attr => attr.Name == name ||
                    (attr.Name.StartsWith("^") && Regex.Match(name, attr.Name).Success))
                .Any()).ToList())
            {
                try
                {
                    action.Invoke(name, from, to);
                }
                catch (Exception e)
                {
                    this.Log.Error($"Unhandled exception while handling changed int variable '{name}': {e.Message}");
                }
            }
        }

        /// <summary>
        /// Invoked when a <see cref="string"/> variable changed.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="from">The old value of the variable.</param>
        /// <param name="to">The new value of the variable.</param>
        /// <param name="internalID">The internal GUID of the variable.</param>
        private void TextVariableChanged(string name, string? from, string? to, Guid? internalID = null)
        {
            if (name == $"{this.Name}.loglevel#")
            {
                try
                {
                    this.Log.SetLogLevel(to);
                }
                catch (ArgumentException)
                {
                    this.Log.Error($"Error setting log level: '{to!}' is not a valid log level.");
                }
            }

            foreach (Action<string, string?, string?> action in this.StringChangedHandlers.Where(
                action => action.Method.GetCustomAttributes<StringAttribute>().Where(
                    attr => attr.Name == name ||
                    (attr.Name.StartsWith("^") && Regex.Match(name, attr.Name).Success))
                .Any()).ToList())
            {
                try
                {
                    action.Invoke(name, from, to);
                }
                catch (Exception e)
                {
                    this.Log.Error($"Unhandled exception while handling changed string variable '{name}': {e.Message}");
                }
            }
        }

        /// <summary>
        /// A list of event handlers (Actions). Basically just a list that
        /// implements the + and - operators because they look nice.
        /// </summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        protected class HandlerList<T> : List<T>
        {
            /// <summary>
            /// Adds a handler to the list.
            /// </summary>
            /// <param name="handlers">The list to add to.</param>
            /// <param name="item">The handler to add.</param>
            /// <returns>The sum of both.</returns>
            public static HandlerList<T> operator +(HandlerList<T> handlers, T item)
            {
                handlers.Add(item);
                return handlers;
            }

            /// <summary>
            /// Removes a handler from the list.
            /// </summary>
            /// <param name="handlers">The list to remove from.</param>
            /// <param name="item">The handler to remove.</param>
            /// <returns>The list without the handler.</returns>
            public static HandlerList<T> operator -(HandlerList<T> handlers, T item)
            {
                handlers.Remove(item);
                return handlers;
            }
        }

        /// <summary>
        /// Denotes a handler for <see
        /// cref="VaInit1(VoiceAttackInitProxyClass)"/>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class InitAttribute : Attribute
        {
        }

        /// <summary>
        /// Denotes a handler for <see
        /// cref="VaExit1(VoiceAttackProxyClass)"/>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class ExitAttribute : Attribute
        {
        }

        /// <summary>
        /// Denotes a handler for <see cref="VaStopCommand()"/>.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class StopCommandAttribute : Attribute
        {
        }

        /// <summary>
        /// Denotes a handler for a plugin contexts.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        protected class ContextAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ContextAttribute"/>
            /// class.
            /// </summary>
            /// <param name="name">The name of or regex for the context.</param>
            public ContextAttribute(string name)
            {
                this.Name = name.ToLower();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ContextAttribute"/>
            /// class that will be invoked for all contexts.
            /// </summary>
            public ContextAttribute()
            {
                this.Name = "^.*";
            }

            /// <summary>
            /// Gets the name of or regex for the context.
            /// </summary>
            public string Name { get; }
        }

        /// <summary>
        /// Denotes a handler for changed <see cref="bool"/> variables.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        protected class BoolAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BoolAttribute"/>
            /// class.
            /// </summary>
            /// <param name="name">The name of or regex for the variable.</param>
            public BoolAttribute(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BoolAttribute"/>
            /// class that will be invoked for all variables.
            /// </summary>
            public BoolAttribute()
            {
                this.Name = "^.*";
            }

            /// <summary>
            /// Gets the name of or regex for the variable name.
            /// </summary>
            public string Name { get; }
        }

        /// <summary>
        /// Denotes a handler for changed <see cref="DateTime"/> variables.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        protected class DateTimeAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see
            /// cref="DateTimeAttribute"/> class.
            /// </summary>
            /// <param name="name">The name of or regex for the variable.</param>
            public DateTimeAttribute(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see
            /// cref="DateTimeAttribute"/> class that will be invoked for all
            /// variables.
            /// </summary>
            public DateTimeAttribute()
            {
                this.Name = "^.*";
            }

            /// <summary>
            /// Gets the name of or regex for the variable.
            /// </summary>
            public string Name { get; }
        }

        /// <summary>
        /// Denotes a handler for changed <see cref="decimal"/> variables.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        protected class DecimalAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DecimalAttribute"/>
            /// class.
            /// </summary>
            /// <param name="name">The name of or regex for the variable.</param>
            public DecimalAttribute(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DecimalAttribute"/>
            /// class that will be invoked for all variables.
            /// </summary>
            public DecimalAttribute()
            {
                this.Name = "^.*";
            }

            /// <summary>
            /// Gets the name of or regex for the variable.
            /// </summary>
            public string Name { get; }
        }

        /// <summary>
        /// Denotes a handler for changed <see cref="int"/> variables.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        protected class IntAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="IntAttribute"/>
            /// class.
            /// </summary>
            /// <param name="name">The name of or regex for the variable.</param>
            public IntAttribute(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="IntAttribute"/>
            /// class that will be invoked for all variables.
            /// </summary>
            public IntAttribute()
            {
                this.Name = "^.*";
            }

            /// <summary>
            /// Gets the name of or regex for the variable.
            /// </summary>
            public string Name { get; }
        }

        /// <summary>
        /// Denotes a handler for changed <see cref="string"/> variables.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        protected class StringAttribute : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StringAttribute"/>
            /// class.
            /// </summary>
            /// <param name="name">The name of or regex for the variable.</param>
            public StringAttribute(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="StringAttribute"/>
            /// class that is invoked for all variables.
            /// </summary>
            public StringAttribute()
            {
                this.Name = "^.*";
            }

            /// <summary>
            /// Gets the name of or regex for the variable.
            /// </summary>
            public string Name { get; }
        }
    }
}
