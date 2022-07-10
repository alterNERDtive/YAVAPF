// <copyright file="VoiceAttackLog.cs" company="alterNERDtive">
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

namespace alterNERDtive.Yavapf
{
    /// <summary>
    /// Exposes methods for writing to the VoiceAttack event log.
    /// </summary>
    public class VoiceAttackLog
    {
        private static readonly string[] LogColour = { "red", "yellow", "green", "blue", "gray" };

        private static LogLevel? logLevel;

        private readonly VoiceAttackInitProxyClass vaProxy;
        private readonly string id;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceAttackLog"/> class.
        /// </summary>
        /// <param name="vaProxy">A VoiceAttackInitProxyClass object to interface with.</param>
        /// <param name="name">The plugin name.</param>
        public VoiceAttackLog(VoiceAttackInitProxyClass vaProxy, string name)
        {
            this.vaProxy = vaProxy;
            this.id = name;
        }

        /// <summary>
        /// Gets or sets the current log level. Default is NOTICE.
        /// </summary>
        public LogLevel? LogLevel
        {
            get => logLevel ?? Yavapf.LogLevel.NOTICE;
            set
            {
                if (value != logLevel)
                {
                    logLevel = value;
                    this.vaProxy.SetText($"{this.id}.loglevel#", value.ToString().ToLower());
                    this.Notice($"Log level set to {value ?? Yavapf.LogLevel.NOTICE}.");
                }
            }
        }

        /// <summary>
        /// Sets the current log level.
        ///
        /// Valid values are ERROR, WARN, NOTICE, INFO and DEBUG.
        /// </summary>
        /// <param name="level">The new log level.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref
        /// name="level"/>is not a valid log level.</exception>
        public void SetLogLevel(string? level)
        {
            if (level == null)
            {
                this.LogLevel = null;
            }
            else
            {
                this.LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), level.ToUpper());
            }
        }

        /// <summary>
        /// Logs a given message with the ERROR log level.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public void Error(string message) => this.Log(message, Yavapf.LogLevel.ERROR);

        /// <summary>
        /// Logs a given message with the WARN log level.
        ///
        /// If the current log level is ERROR the message will not be logged.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public void Warn(string message) => this.Log(message, Yavapf.LogLevel.WARN);

        /// <summary>
        /// Logs a given message with the NOTICE log level.
        ///
        /// If the current log level is ERROR or WARN the message will not be
        /// logged.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public void Notice(string message) => this.Log(message, Yavapf.LogLevel.NOTICE);

        /// <summary>
        /// Logs a given message with the INFO log level.
        ///
        /// If the current log level is ERROR, WARN or NOTICE the message will
        /// not be logged.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public void Info(string message) => this.Log(message, Yavapf.LogLevel.INFO);

        /// <summary>
        /// Logs a given message with the DEBUG log level.
        ///
        /// If the current log level is not DEBUG the message will not be logged.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public void Debug(string message) => this.Log(message, Yavapf.LogLevel.DEBUG);

        /// <summary>
        /// Logs a given message with the specified log level.
        ///
        /// If the current log level is higher than the message’s log level it
        /// will not be logged.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="level">The desired log level.</param>
        /// <exception cref="ArgumentNullException">Thrown when the message is null.</exception>
        private void Log(string message, LogLevel level = Yavapf.LogLevel.INFO)
        {
            _ = message ?? throw new ArgumentNullException("message");

            if (level <= this.LogLevel)
            {
                this.vaProxy.WriteToLog($"{level} | {this.id}: {message}", LogColour[(int)level]);
            }
        }
    }

    /// <summary>
    /// Log levels that can be used when writing to the VoiceAttack event log.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Well it’s this or wrong file name, isn’t it?")]
    public enum LogLevel
    {
        /// <summary>
        /// Log level for error messages. Errors cause execution to abort.
        /// </summary>
        ERROR,

        /// <summary>
        /// Log level for warning messages. Warnings should not cause execution
        /// to abort.
        /// </summary>
        WARN,

        /// <summary>
        /// Log level for notices. Notices should be noteworthy.
        /// </summary>
        NOTICE,

        /// <summary>
        /// Log level for informational messages. These should not be
        /// noteworthy.
        /// </summary>
        INFO,

        /// <summary>
        /// Log level for debug messages. They should be useful only for
        /// debugging.
        /// </summary>
        DEBUG,
    }
}
