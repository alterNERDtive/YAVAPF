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
    public class VoiceAttackLog
    {
        private static readonly string[] LogColour = { "red", "yellow", "green", "blue", "gray" };

        private static LogLevel? logLevel;

        private readonly VoiceAttackInitProxyClass vaProxy;
        private readonly string id;

        public VoiceAttackLog(VoiceAttackInitProxyClass vaProxy, string id)
        {
            this.vaProxy = vaProxy;
            this.id = id;
        }

        public LogLevel? LogLevel
        {
            get => logLevel ?? Yavapf.LogLevel.NOTICE;
            set
            {
                logLevel = value;
                this.Notice($"Log level set to {value ?? Yavapf.LogLevel.NOTICE}.");
            }
        }

        public void SetLogLevel(string level)
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

        public void Log(string message, LogLevel level = Yavapf.LogLevel.INFO)
        {
            _ = message ?? throw new ArgumentNullException("message");

            if (level <= this.LogLevel)
            {
                this.vaProxy.WriteToLog($"{level} | {this.id}: {message}", LogColour[(int)level]);
            }
        }

        public void Error(string message) => this.Log(message, Yavapf.LogLevel.ERROR);

        public void Warn(string message) => this.Log(message, Yavapf.LogLevel.WARN);

        public void Notice(string message) => this.Log(message, Yavapf.LogLevel.NOTICE);

        public void Info(string message) => this.Log(message, Yavapf.LogLevel.INFO);

        public void Debug(string message) => this.Log(message, Yavapf.LogLevel.DEBUG);
    }

    /// <summary>
    /// Log levels that can be used when writing to the VoiceAttack log.
    /// </summary>
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
