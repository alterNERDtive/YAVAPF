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
        /// Initializes a new instance of the <see cref="VoiceAttackPlugin"/> class.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="version">The current version of the plugin.</param>
        /// <param name="info">The description of the plugin.</param>
        /// <param name="guid">The GUID of the plugin.</param>
        public VoiceAttackPlugin(string name, string version, string info, string guid)
            => (this.Name, this.Version, this.Info, this.Guid)
            = (name, version, info, new (guid));

        /// <summary>
        /// Invoked when VoiceAttack initializes the plugin.
        /// </summary>
        public event Action<VoiceAttackInitProxyClass>? Init;

        /// <summary>
        /// Invoked when VoiceAttack closes.
        /// </summary>
        public event Action<VoiceAttackProxyClass>? Exit;

        /// <summary>
        /// Invoked when VoiceAttack stops all commands.
        /// </summary>
        public event Action? Stop;

        /// <summary>
        /// Gets or sets the Actions to be run when the plugin is invoked from a
        /// VoiceAttack command. Only Actions with a matching “Context”
        /// attribute will be invoked.
        /// </summary>
        public HandlerList<Action<VoiceAttackInvokeProxyClass>> Contexts { get; set; } = new ();

        /// <summary>
        /// Gets the currently stored VoiceAttackInitProxyClass object which is
        /// used to interface with VoiceAttack.
        ///
        /// You will usually want to use the provided methods and Properties
        /// instead.
        /// </summary>
        public VoiceAttackInitProxyClass Proxy
        {
            get => this.vaProxy!;
        }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the current version of the plugin.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the description of the plugin.
        /// </summary>
        public string Info { get; }

        /// <summary>
        /// Gets the GUID of the plugin.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Gets the <see cref="VoiceAttackLog"/> instance the plugin uses to
        /// log to the VoiceAttack event log.
        /// 
        /// You can use this to log your own messages.
        /// </summary>
        public VoiceAttackLog Log
        {
            get => this.log ??= new VoiceAttackLog(this.Proxy, this.Name);
        }

        /// <summary>
        /// Gets the value of a variable from VoiceAttack.
        ///
        /// Valid varible types are <see cref="bool"/>, <see cref="DateTime"/>,
        /// <see cref="decimal"/>, <see cref="int"/>, <see cref="short"/> and
        /// <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The value of the variable. Can be null.</returns>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        public T? Get<T>(string name)
        {
            dynamic? ret = typeof(T) switch
            {
                Type boolType when boolType == typeof(bool) => this.Proxy.GetBoolean(name),
                Type dateType when dateType == typeof(DateTime) => this.Proxy.GetDate(name),
                Type decType when decType == typeof(decimal) => this.Proxy.GetDecimal(name),
                Type intType when intType == typeof(int) => this.Proxy.GetInt(name),
                Type shortType when shortType == typeof(short) => this.Proxy.GetSmallInt(name),
                Type stringType when stringType == typeof(string) => this.Proxy.GetText(name),
                _ => throw new InvalidDataException($"Cannot get variable '{name}': invalid type '{typeof(T).Name}'."),
            };
            return ret;
        }

        /// <summary>
        /// Sets a variable for use in VoiceAttack.
        ///
        /// Valid varible types are <see cref="bool"/>, <see cref="DateTime"/>,
        /// <see cref="decimal"/>, <see cref="int"/>, <see cref="short"/> and
        /// <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable. Can not be null.</param>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        public void Set<T>(string name, T? value)
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
                    case short s:
                        this.Proxy.SetSmallInt(name, s);
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
        /// <see cref="decimal"/>, <see cref="int"/>, <see cref="short"/> and
        /// <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="name">The name of the variable.</param>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        public void Unset<T>(string name)
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
                case Type shortType when shortType == typeof(short):
                    this.Proxy.SetSmallInt(name, null);
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
        public string VA_DisplayName() => $"{this.Name} v{this.Version}";

        /// <summary>
        /// The plugin’s description, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The description.</returns>
        public string VA_DisplayInfo() => this.Info;

        /// <summary>
        /// The plugin’s GUID, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The GUID.</returns>
        public Guid VA_Id() => this.Guid;

        /// <summary>
        /// The Init method, as required by the VoiceAttack plugin API.
        /// Runs when the plugin is initially loaded.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public void VA_Init1(VoiceAttackInitProxyClass vaProxy)
        {
            this.vaProxy = vaProxy;

            this.Set<string>($"{this.Name}.version", this.Version);
            this.Log.Debug($"Initializing v{this.Version} …");

            this.vaProxy.TextVariableChanged += this.TextVariableChanged;
            this.vaProxy.IntegerVariableChanged += this.IntegerVariableChanged;
            this.vaProxy.DecimalVariableChanged += this.DecimalVariableChanged;
            this.vaProxy.BooleanVariableChanged += this.BooleanVariableChanged;
            this.vaProxy.DateVariableChanged += this.DateVariableChanged;

            this.Log.Debug("Running Init handlers …");
            this.Init?.Invoke(vaProxy);
            this.Log.Debug("Finished running Init handlers.");

            this.Set<bool>($"{this.Name}.initialized", true);
            this.Log.Debug("Initialized.");
        }

        /// <summary>
        /// The Invoke method, as required by the VoiceAttack plugin API.
        /// Runs whenever a plugin context is invoked.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public void VA_Invoke1(VoiceAttackInvokeProxyClass vaProxy)
        {
            this.vaProxy = vaProxy;

            string context = vaProxy.Context.ToLower();

            try
            {
                bool exists = false;
                foreach (Action<VoiceAttackInvokeProxyClass> action in this.Contexts)
                {
                    foreach (ContextAttribute attr in action.Method.GetCustomAttributes<ContextAttribute>())
                    {
                        if (attr.Name == context ||
                            (attr.Name.StartsWith("^") && Regex.Match(context, attr.Name).Success))
                        {
                            exists = true;
                            action.Invoke(vaProxy);

                            // Make sure that we don’t run the same Action
                            // multiple times just because it has e.g. multiple
                            // matching context regexes.
                            break;
                        }
                    }
                }

                if (!exists)
                {
                    this.Log.Error($"Invalid plugin context '{vaProxy.Context}'.");
                }
            }
            catch (ArgumentNullException e)
            {
                this.Log.Error($"Missing parameter '{e.ParamName}' for context '{context}'");
            }
            catch (Exception e)
            {
                this.Log.Error($"Unhandled exception while executing plugin context '{context}'. ({e.Message})");
            }
        }

        /// <summary>
        /// The Exit method, as required by the VoiceAttack plugin API.
        /// Runs when VoiceAttack is shut down.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public void VA_Exit1(VoiceAttackProxyClass vaProxy)
        {
            this.Exit?.Invoke(vaProxy);
        }

        /// <summary>
        /// The StopCommand method, as required by the VoiceAttack plugin API.
        /// Runs whenever all commands are stopped using the “Stop All Commands”
        /// button or action.
        /// </summary>
        public void VA_StopCommand()
        {
            this.Stop?.Invoke();
        }

        protected void TextVariableChanged(string name, string from, string to, Guid? internalID = null)
        {
        }

        protected void IntegerVariableChanged(string name, int? from, int? to, Guid? internalID = null)
        {
        }

        protected void DecimalVariableChanged(string name, decimal? from, decimal? to, Guid? internalID = null)
        {
        }

        protected void BooleanVariableChanged(string name, bool? from, bool? to, Guid? internalID = null)
        {
        }

        protected void DateVariableChanged(string name, DateTime? from, DateTime? to, Guid? internalID = null)
        {
        }

        public class HandlerList<T> : List<T>
        {
            public static HandlerList<T> operator +(HandlerList<T> handlers, T item)
            {
                handlers.Add(item);
                return handlers;
            }

            public static HandlerList<T> operator -(HandlerList<T> handlers, T item)
            {
                handlers.Remove(item);
                return handlers;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ContextAttribute : Attribute
    {
        public string Name { get; }

        public ContextAttribute(string context)
        {
            this.Name = context;
        }
    }
}
