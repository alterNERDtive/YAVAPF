// <copyright file="VoiceAttackInitProxyExtension.cs" company="alterNERDtive">
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
    /// Extends the <see cref="VoiceAttackInitProxyClass"/> class with generic
    /// methods for getting and setting VoiceAttack variables.
    /// </summary>
    public static class VoiceAttackInitProxyExtension
    {
        /// <summary>
        /// Gets the value of a variable from VoiceAttack.
        ///
        /// Valid varible types are <see cref="bool"/>, <see cref="DateTime"/>,
        /// <see cref="decimal"/>, <see cref="int"/> and <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="vaProxy">The <see cref="VoiceAttackInitProxyClass"/> object.</param>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The value of the variable. Can be null.</returns>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        public static T? Get<T>(this VoiceAttackInitProxyClass vaProxy, string name)
        {
            dynamic? ret = typeof(T) switch
            {
                Type boolType when boolType == typeof(bool) => vaProxy.GetBoolean(name),
                Type dateType when dateType == typeof(DateTime) => vaProxy.GetDate(name),
                Type decType when decType == typeof(decimal) => vaProxy.GetDecimal(name),
                Type intType when intType == typeof(int) => vaProxy.GetInt(name),
                Type stringType when stringType == typeof(string) => vaProxy.GetText(name),
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
        /// <param name="vaProxy">The <see cref="VoiceAttackInitProxyClass"/> object.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable. Can not be null.</param>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        public static void Set<T>(this VoiceAttackInitProxyClass vaProxy, string name, T? value)
        {
            if (value == null)
            {
                vaProxy.Unset<T>(name);
            }
            else
            {
                switch (value)
                {
                    case bool b:
                        vaProxy.SetBoolean(name, b);
                        break;
                    case DateTime d:
                        vaProxy.SetDate(name, d);
                        break;
                    case decimal d:
                        vaProxy.SetDecimal(name, d);
                        break;
                    case int i:
                        vaProxy.SetInt(name, i);
                        break;
                    case string s:
                        vaProxy.SetText(name, s);
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
        /// <param name="vaProxy">The <see cref="VoiceAttackInitProxyClass"/> object.</param>
        /// <param name="name">The name of the variable.</param>
        /// <exception cref="InvalidDataException">Thrown when the variable is of an invalid type.</exception>
        public static void Unset<T>(this VoiceAttackInitProxyClass vaProxy, string name)
        {
            switch (typeof(T))
            {
                case Type boolType when boolType == typeof(bool):
                    vaProxy.SetBoolean(name, null);
                    break;
                case Type dateType when dateType == typeof(DateTime):
                    vaProxy.SetDate(name, null);
                    break;
                case Type decType when decType == typeof(decimal):
                    vaProxy.SetDecimal(name, null);
                    break;
                case Type intType when intType == typeof(int):
                    vaProxy.SetInt(name, null);
                    break;
                case Type stringType when stringType == typeof(string):
                    vaProxy.SetText(name, null);
                    break;
                default:
                    throw new InvalidDataException($"Cannot set variable '{name}': invalid type '{typeof(T).Name}'.");
            }
        }
    }
}
