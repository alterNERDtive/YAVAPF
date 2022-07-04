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

namespace alterNERDtive.Yavapf
{
    public class VoiceAttackPlugin
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string Info { get; set; }

        public string Guid { get; set; }

        private dynamic? vaProxy;

        public string VA_DisplayName() => $"{this.Name} v{this.Version}";

        public string VA_DisplayInfo() => this.Info;

        public Guid VA_Id() => new (this.Guid);

        public void VA_Init1(dynamic vaProxy)
        {
            this.vaProxy = vaProxy;
            this.vaProxy.TextVariableChanged += new Action<string, string, string, Guid?>(this.TextVariableChanged);
            this.vaProxy.IntegerVariableChanged += new Action<string, int?, int?, Guid?>(this.IntegerVariableChanged);
            this.vaProxy.DecimalVariableChanged += new Action<string, decimal?, decimal?, Guid?>(this.DecimalVariableChanged);
            this.vaProxy.BooleanVariableChanged += new Action<string, bool?, bool?, Guid?>(this.BooleanVariableChanged);
            this.vaProxy.DateVariableChanged += new Action<string, DateTime?, DateTime?, Guid?>(this.DateVariableChanged);
        }

        public void VA_Invoke1(dynamic vaProxy)
        {
            this.vaProxy = vaProxy;
        }

        public void VA_Exit1(dynamic vaProxy)
        {
            this.vaProxy = vaProxy;
        }

        public void VA_StopCommand()
        {
        }

        private void TextVariableChanged(string name, string from, string to, Guid? internalID = null)
        { }

        private void IntegerVariableChanged(string name, int? from, int? to, Guid? internalID = null)
        { }

        private void DecimalVariableChanged(string name, decimal? from, decimal? to, Guid? internalID = null)
        { }

        private void BooleanVariableChanged(string name, bool? from, bool? to, Guid? internalID = null)
        { }

        private void DateVariableChanged(string name, DateTime? from, DateTime? to, Guid? internalID = null)
        { }
    }
}
