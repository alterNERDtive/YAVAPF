// <copyright file="ExamplePlugin.cs" company="alterNERDtive">
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

using VoiceAttack;

namespace alterNERDtive.Yavapf.Example
{
    public class ExamplePlugin
    {
        private static readonly VoiceAttackPlugin Plugin;

        static ExamplePlugin()
        {
            Plugin = new (
                name: "Example Plugin",
                version: "0.0.1",
                info: "This is a description",
                guid: "{76FE674F-F729-45FD-A1DD-E53E9E66B360}");

            Plugin.Init += Init;
            Plugin.Exit += Exit;
            Plugin.Stop += Stop;

            Plugin.Contexts += Test;
        }

        public static string VA_DisplayName() => Plugin.VA_DisplayName();

        public static string VA_DisplayInfo() => Plugin.VA_DisplayInfo();

        public static Guid VA_Id() => Plugin.VA_Id();

        public static void VA_Init1(dynamic vaProxy) => Plugin.VA_Init1(vaProxy);

        public static void VA_Invoke1(dynamic vaProxy) => Plugin.VA_Invoke1(vaProxy);

        public static void VA_Exit1(dynamic vaProxy) => Plugin.VA_Exit1(vaProxy);

        public static void VA_StopCommand() => Plugin.VA_StopCommand();

        private static void Init(VoiceAttackInitProxyClass vaProxy)
        {
        }

        private static void Exit(VoiceAttackProxyClass vaProxy)
        {
        }

        private static void Stop()
        {
        }

        [Context("test")]
        [Context("other name")]
        private static void Test(VoiceAttackInvokeProxyClass vaProxy)
        {
            Plugin.Log.Notice($"Plugin context '{vaProxy.Context}' invoked.");
        }
    }
}
