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
using VoiceAttack;

namespace alterNERDtive.Yavapf
{
    public class VoiceAttackPlugin
    {
        public VoiceAttackPlugin(string name, string version, string info, string guid)
            => (this.Name, this.Version, this.Info, this.Guid)
            = (name, version, info, new (guid));

        // this just hides the default constructor to make sure Properties are set.
        private VoiceAttackPlugin()
        {
        }

        public event Action<VoiceAttackInitProxyClass>? Init;

        public event Action<VoiceAttackProxyClass>? Exit;

        public event Action? Stop;

        public HandlerList<Action<VoiceAttackInvokeProxyClass>> Contexts { get; set; } = new ();

        public VoiceAttackInitProxyClass Proxy
        {
            get => this.vaProxy;
        }

        public string Name { get; }

        public string Version { get; }

        public string Info { get; }

        public Guid Guid { get; }

        public VoiceAttackLog Log {
            get => this.log ??= new VoiceAttackLog(this.vaProxy, this.Name);
        }

        private VoiceAttackLog log;

        protected VoiceAttackInitProxyClass vaProxy;

        public void Set<T>(string name, T? value)
        {
            switch (value)
            {
                case bool b:
                    this.vaProxy.SetBoolean(name, b);
                    break;
                case DateTime d:
                    this.vaProxy.SetDate(name, d);
                    break;
                case decimal d:
                    this.vaProxy.SetDecimal(name, d);
                    break;
                case int i:
                    this.vaProxy.SetInt(name, i);
                    break;
                case short s:
                    this.vaProxy.SetSmallInt(name, s);
                    break;
                case string s:
                    this.vaProxy.SetText(name, s);
                    break;
                default:
                    throw new InvalidDataException($"Cannot set variable '{name}': invalid type '{typeof(T).Name}'.");
            }
        }

        public T? Get<T>(string name)
        {
            dynamic? ret = null;

            switch (default(T))
            {
                case bool _:
                    ret = this.vaProxy.GetBoolean(name);
                    break;
                case DateTime _:
                    ret = this.vaProxy.GetDate(name);
                    break;
                case decimal _:
                    ret = this.vaProxy.GetDecimal(name);
                    break;
                case int _:
                    ret = this.vaProxy.GetInt(name);
                    break;
                case short _:
                    ret = this.vaProxy.GetSmallInt(name);
                    break;
                case string _:
                    ret = this.vaProxy.GetText(name);
                    break;
                default:
                    throw new InvalidDataException($"Cannot get variable '{name}': invalid type '{typeof(T).Name}'.");
            }

            return ret;
        }

        public string VA_DisplayName() => $"{this.Name} v{this.Version}";

        public string VA_DisplayInfo() => this.Info;

        public Guid VA_Id() => this.Guid;

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

        public void VA_Invoke1(VoiceAttackInvokeProxyClass vaProxy)
        {
            this.vaProxy = vaProxy;

            string context = vaProxy.Context.ToLower();

            try
            {
                bool exists = false;
                foreach (Action<VoiceAttackInvokeProxyClass> action in Contexts)
                {
                    foreach (ContextAttribute attr in action.Method.GetCustomAttributes<ContextAttribute>())
                    {
                        if (attr.Name == context)
                        {
                            exists = true;
                            action.Invoke(vaProxy);
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

        public void VA_Exit1(VoiceAttackProxyClass vaProxy)
        {
            this.Exit?.Invoke(vaProxy);
        }

        public void VA_StopCommand()
        {
            this.Stop?.Invoke();
        }

        private void TextVariableChanged(string name, string from, string to, Guid? internalID = null)
        {
        }

        private void IntegerVariableChanged(string name, int? from, int? to, Guid? internalID = null)
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
