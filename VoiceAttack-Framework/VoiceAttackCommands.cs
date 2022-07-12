// <copyright file="VoiceAttackCommands.cs" company="alterNERDtive">
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

using VoiceAttack;

namespace alterNERDtive.Yavapf
{
    /// <summary>
    /// Provides an interface to run VoiceAttack commands from a plugin.
    /// </summary>
    public class VoiceAttackCommands
    {
        private readonly VoiceAttackInitProxyClass vaProxy;
        private readonly VoiceAttackLog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceAttackCommands"/>
        /// class.
        /// </summary>
        /// <param name="vaProxy">The <see cref="VoiceAttackInitProxyClass"/>
        /// proxy object.</param>
        /// <param name="log">The <see cref="VoiceAttackLog"/> object.</param>
        internal VoiceAttackCommands(VoiceAttackInitProxyClass vaProxy, VoiceAttackLog log) => (this.vaProxy, this.log) = (vaProxy, log);

        /// <summary>
        /// Runs a VoiceAttack command.
        /// </summary>
        /// <param name="command">The name of the command.</param>
        /// <param name="logMissing">Whether or not to log a message if the
        /// command in question does not exist in the current profile.</param>
        /// <param name="wait">Whether to wait for the command to finish
        /// executing before returning.</param>
        /// <param name="subcommand">Whether the called command should be run as
        /// a subcommand to the current command context.</param>
        /// <param name="parameters">The parameters for the command. Has to be
        /// an array of arrays, with the inner arrays being of a valid
        /// Voiceattack variable type.</param>
        /// <exception cref="ArgumentNullException">Thrown if the name of the
        /// command is missing.</exception>
        public void Run(string command, bool logMissing = true, bool wait = false, bool subcommand = false, dynamic[]? parameters = null)
        {
            _ = command ?? throw new ArgumentNullException("command");

            if (this.vaProxy.Command.Exists(command))
            {
                this.log.Debug($"Parsing arguments for command '{command}' …");

                string[]? strings = null;
                int[]? integers = null;
                decimal[]? decimals = null;
                bool[]? booleans = null;
                DateTime[]? dates = null; // this might not work!

                foreach (var values in parameters ?? Enumerable.Empty<object>())
                {
                    switch (values)
                    {
                        case bool[] b:
                            booleans = b;
                            break;
                        case DateTime[] d:
                            dates = d;
                            break;
                        case decimal[] d:
                            decimals = d;
                            break;
                        case int[] i:
                            integers = i;
                            break;
                        case string[] s:
                            strings = s;
                            break;
                        default:
                            break;
                    }
                }

                this.log.Debug($"Running command '{command}' …");

                this.vaProxy.Command.Execute(
                    CommandPhrase: command,
                    WaitForReturn: wait,
                    AsSubcommand: subcommand,
                    CompletedAction: null,
                    PassedText: strings == null ? null : $"\"{string.Join<string>("\";\"", strings)}\"",
                    PassedIntegers: integers == null ? null : string.Join<int>(";", integers),
                    PassedDecimals: decimals == null ? null : string.Join<decimal>(";", decimals),
                    PassedBooleans: booleans == null ? null : string.Join<bool>(";", booleans),
                    PassedDates: dates == null ? null : string.Join<DateTime>(";", dates));
            }
            else
            {
                if (logMissing)
                {
                    this.log.Warn($"Tried running missing command '{command}'.");
                }
            }
        }

        /// <summary>
        /// Runs a VoiceAttack command with a given list of prefixes. Will run
        /// `prefix.command` for each prefix.
        /// </summary>
        /// <param name="prefixes">The list of prefixes.</param>
        /// <param name="command">The name of the command.</param>
        /// <param name="logMissing">Whether or not to log a message if the
        /// command in question does not exist in the current profile.</param>
        /// <param name="wait">Whether to wait for the command to finish
        /// executing before returning.</param>
        /// <param name="subcommand">Whether the called command should be run as
        /// a subcommand to the current command context.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <exception cref="ArgumentNullException">Thrown if the name of the
        /// command is missing.</exception>
        public void RunAll(IEnumerable<string> prefixes, string command, bool logMissing = true, bool wait = false, bool subcommand = false, dynamic[][]? parameters = null)
        {
            foreach (string prefix in prefixes)
            {
                this.Run($"{prefix}.{command}", logMissing, wait, subcommand, parameters);
            }
        }

        /// <summary>
        /// Runs a VoiceAttack event command. Event commands are enclosed in
        /// double paratheses by convention, they will be added automatically.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="logMissing">Whether or not to log a message if the
        /// command in question does not exist in the current profile.</param>
        /// <param name="wait">Whether to wait for the command to finish
        /// executing before returning.</param>
        /// <param name="subcommand">Whether the called command should be run as
        /// a subcommand to the current command context.</param>
        /// <param name="parameters">The parameters for the command. Has to be
        /// an array of arrays, with the inner arrays being of a valid
        /// Voiceattack variable type.</param>
        /// <exception cref="ArgumentNullException">Thrown if the name of the
        /// command is missing.</exception>
        public void TriggerEvent(string name, bool logMissing = true, bool wait = false, bool subcommand = false, dynamic[]? parameters = null)
        {
            this.Run($"(({name}))", logMissing, wait, subcommand, parameters);
        }

        /// <summary>
        /// Runs a VoiceAttack event command with a given list of prefixes.
        /// Event commands are enclosed in double paratheses by convention, they
        /// will be added automatically. Will run `((prefix.name))` for each
        /// prefix.
        /// </summary>
        /// <param name="prefixes">The list of prefixes.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="logMissing">Whether or not to log a message if the
        /// command in question does not exist in the current profile.</param>
        /// <param name="wait">Whether to wait for the command to finish
        /// executing before returning.</param>
        /// <param name="subcommand">Whether the called command should be run as
        /// a subcommand to the current command context.</param>
        /// <param name="parameters">The parameters for the command. Has to be
        /// an array of arrays, with the inner arrays being of a valid
        /// Voiceattack variable type.</param>
        /// <exception cref="ArgumentNullException">Thrown if the name of the
        /// command is missing.</exception>
        public void TriggerEventAll(IEnumerable<string> prefixes, string name, bool logMissing = true, bool wait = false, bool subcommand = false, dynamic[][]? parameters = null)
        {
            foreach (string prefix in prefixes)
            {
                this.Run($"(({prefix}.{name}))", logMissing, wait, subcommand, parameters);
            }
        }
    }
}
