/*
 * Copyright 2018 Equilateral IT
 *
 * ConnectorLib is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ConnectorLib is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with ConnectorLib.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace ConnectorLibDemo
{
    /// <summary>
    /// Application settings.
    /// Most of the values in this class are persisted outside of the DB.
    /// </summary>
    internal static class Settings
    {
        [NotNull] private static string RegistryRoot => @"Software\Equilateral IT\ConnectorLibDemo\";

        [NotNull]
        private static T GetRegistry<T>([NotNull] string name, [NotNull] T defaultValue = default(T))
        {
            try
            {
                //object retval = Application.UserAppDataRegistry.GetValue(name, defaultValue);
                using (RegistryKey key = Registry.CurrentUser?.OpenSubKey(RegistryRoot))
                {
                    if (key == null) { return defaultValue; }
                    object retval = key.GetValue(name, defaultValue);
                    if (retval?.GetType().IsAssignableFrom(typeof(T)) ?? false) { return (T)retval; }
                    T result = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString((string)retval);
                    return (result == null) ? defaultValue : result; // null coalesce doesn't work here - kkat
                }
            }
            catch { return defaultValue; }
        }

        private static void SetRegistry<T>([NotNull] string name, [NotNull] T value)
        {
            //try { Application.UserAppDataRegistry.SetValue(name, value); }
            try { using (RegistryKey key = Registry.CurrentUser?.CreateSubKey(RegistryRoot)) { key?.SetValue(name, value); } }
            catch (Exception ex) { BugReport.LogBug(ex); }
        }

        public static Connector.Interface ConnectorInterface
        {
            get => GetRegistry("connectorInterface", Connector.Interface.LuaConnector);
            set => SetRegistry("connectorInterface", value);
        }

        public static Connector.LuaSocketType ConnectorLuaSocketType
        {
            get => GetRegistry("connectorLuaSocketType", Connector.LuaSocketType.Local);
            set => SetRegistry("connectorLuaSocketType", value);
        }

        public static class Connector
        {
            public enum Interface : byte
            {
                LuaConnector = 0,
                // ReSharper disable once InconsistentNaming
                sd2snes = 1
            }

            public enum LuaSocketType : byte
            {
                Local = 0,
                Internet = 1
            }
        }
    }
}