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
    /// <summary>
    /// This is an example for a VoiceAttack plugin using YAVAPF.
    ///
    /// You can use this class and this project as base for your own implementation.
    /// </summary>
    public class ExamplePlugin : VoiceAttackPlugin
    {
        private static readonly ExamplePlugin Plugin;

        /// <summary>
        /// Initializes static members of the <see cref="ExamplePlugin"/> class.
        ///
        /// Since VoiceAttack’s plugin API requires a bunch of static methods
        /// instead of instantiating a plugin class, the “Constructor” here also
        /// needs to be static. It is executed right before a static method is
        /// used for the first time, which would usually be when VoiceAttack
        /// calls the <see cref="VA_Init1(dynamic)"/> method.
        /// </summary>
        static ExamplePlugin()
        {
            // You can generate a GUID in Visual Studio under “Tools” → “Create
            // GUID”. Choose “Registry Format”.
            Plugin = new ()
            {
                Name = "Example Plugin",
                Version = "0.0.1",
                Info = "This is a description",
                Guid = "{76FE674F-F729-45FD-A1DD-E53E9E66B360}",
            };

            // Event handlers are mainly added via attributes, see the end of
            // the file.

            // You can even add lambdas! This one logs an explicit error for
            // invoking the plugin without a context.
            Plugin.Contexts +=
                [Context("")]
                (vaProxy)
                =>
                {
                    Plugin.Log.Error($"Invoking this plugin without a context is not allowed.");
                };
        }

        /// <summary>
        /// The plugin’s display name, as required by the VoiceAttack plugin
        /// API.
        ///
        /// Since it is required to be static, it must be defined in your plugin
        /// class for VoiceAttack to pick it up as a plugin.
        /// </summary>
        /// <returns>The display name.</returns>
        public static string VA_DisplayName() => Plugin.VaDisplayName();

        /// <summary>
        /// The plugin’s description, as required by the VoiceAttack plugin API.
        ///
        /// Since it is required to be static, it must be defined in your plugin
        /// class for VoiceAttack to pick it up as a plugin.
        /// </summary>
        /// <returns>The description.</returns>
        public static string VA_DisplayInfo() => Plugin.VaDisplayInfo();

        /// <summary>
        /// The plugin’s GUID, as required by the VoiceAttack plugin API.
        ///
        /// Since it is required to be static, it must be defined in your plugin
        /// class for VoiceAttack to pick it up as a plugin.
        /// </summary>
        /// <returns>The GUID.</returns>
        public static Guid VA_Id() => Plugin.VaId();

        /// <summary>
        /// The Init method, as required by the VoiceAttack plugin API.
        /// Runs when the plugin is initially loaded.
        ///
        /// Since it is required to be static, it must be defined in your plugin
        /// class for VoiceAttack to pick it up as a plugin.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public static void VA_Init1(dynamic vaProxy) => Plugin.VaInit1(vaProxy);

        /// <summary>
        /// The Invoke method, as required by the VoiceAttack plugin API.
        /// Runs whenever a plugin context is invoked.
        ///
        /// Since it is required to be static, it must be defined in your plugin
        /// class for VoiceAttack to pick it up as a plugin.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public static void VA_Invoke1(dynamic vaProxy) => Plugin.VaInvoke1(vaProxy);

        /// <summary>
        /// The Exit method, as required by the VoiceAttack plugin API.
        /// Runs when VoiceAttack is shut down.
        ///
        /// Since it is required to be static, it must be defined in your plugin
        /// class for VoiceAttack to pick it up as a plugin.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public static void VA_Exit1(dynamic vaProxy) => Plugin.VaExit1(vaProxy);

        /// <summary>
        /// The StopCommand method, as required by the VoiceAttack plugin API.
        /// Runs whenever all commands are stopped using the “Stop All Commands”
        /// button or action.
        ///
        /// Since it is required to be static, it must be defined in your plugin
        /// class for VoiceAttack to pick it up as a plugin.
        /// </summary>
        public static void VA_StopCommand() => Plugin.VaStopCommand();

        /// <summary>
        /// An example handler for VA_Init1. Init handlers are the place to do
        /// anything that your plugin needs to set up when it is initially
        /// loaded at VoiceAttack start.
        /// </summary>
        /// <param name="vaProxy">The current VoiceAttack proxy object.</param>
        [Init]
        public static void Init(VoiceAttackInitProxyClass vaProxy)
        {
            Plugin.Log.Notice("This is the example Init handler method.");
        }

        /// <summary>
        /// An example handler for VA_Exit1. Exit handlers have to make sure
        /// your plugin is properly shut down with VoiceAttack. Do note that
        /// there is a default timeout of 5s.
        /// </summary>
        /// <param name="vaProxy">The current VoiceAttack proxy object.</param>
        [Exit]
        public static void Exit(VoiceAttackProxyClass vaProxy)
        {
            Plugin.Log.Notice("This is the example Exit handler method.");
        }

        /// <summary>
        /// An example handler for VA_StopCommand. If your plugin needs to
        /// execute anything when all commands are stopped this is the place.
        /// </summary>
        [Stop]
        public static void Stop()
        {
            Plugin.Log.Notice("This is the example Stop handler method.");
        }

        /// <summary>
        /// An example handler for plugin contexts. The context(s) must be
        /// specified with “Context” attributes. Contexts must be all lower case.
        ///
        /// This example handles the “test” and “different test” plugin
        /// contexts. They expect that a text parameter “~test” is set in
        /// VoiceAttack.
        /// </summary>
        /// <param name="vaProxy">The current VoiceAttack proxy object.</param>
        [Context("test")]
        [Context("different test")]
        public static void Test(VoiceAttackInvokeProxyClass vaProxy)
        {
            Plugin.Log.Notice(
                $"This is the example handler for the plugin contexts “test” and “different test”. It has been invoked with '{vaProxy.Context}'.");

            string test = Plugin.Get<string?>("~test") ?? throw new ArgumentNullException("~test");
            Plugin.Log.Notice($"The value of 'TXT:~test' is '{test}'");
        }

        /// <summary>
        /// An example handler for plugin contexts. The context(s) must be
        /// specified with “Context” attributes. Contexts must be all lower case.
        ///
        /// This example demonstrates using regular expressions. It handles all
        /// contexts that begin with “foo” or contain “bar”.
        /// </summary>
        /// <param name="vaProxy">The current VoiceAttack proxy object.</param>
        [Context("^foo.*")]
        [Context("^.*bar.*")]
        public static void RegexContext(VoiceAttackInvokeProxyClass vaProxy)
        {
            Plugin.Log.Notice(
                $"This is the example handler for the plugin contexts “^foo.*” and “^.*bar.*”. It has been invoked with '{vaProxy.Context}'.");
        }
    }
}
