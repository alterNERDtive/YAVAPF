// <copyright file="MinimumViablePlugin.cs" company="alterNERDtive">
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

using alterNERDtive.Yavapf;

namespace alterNERDtive.Example
{
    /// <summary>
    /// This is an example for a VoiceAttack plugin using YAVAPF.
    ///
    /// You can use this class and this project as base for your own implementation.
    /// </summary>
    public class MinimumViablePlugin : VoiceAttackPlugin
    {
        private static readonly MinimumViablePlugin Plugin;

        /// <summary>
        /// Initializes static members of the <see cref="MinimumViablePlugin"/> class.
        ///
        /// Since VoiceAttack’s plugin API requires a bunch of static methods
        /// instead of instantiating a plugin class, the “Constructor” here also
        /// needs to be static. It is executed right before a static method is
        /// used for the first time, which would usually be when VoiceAttack
        /// calls the <see cref="VA_Init1(dynamic)"/> method.
        /// </summary>
        static MinimumViablePlugin()
        {
            // You can generate a GUID in Visual Studio under “Tools” → “Create
            // GUID”. Choose “Registry Format”.
            Plugin = new ()
            {
                Name = "Minimum Viable Plugin",
                Version = "0.0.1",
                Info = "This is a description",
                Guid = "{2E5CDD74-0E05-4745-A791-76E8C5AABBC3}",
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
    }
}
