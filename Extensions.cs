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
using System.Collections.Generic;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace ConnectorLib
{
    /// <summary>
    /// Extensions class for convenience methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Adds a value into a dictionary, creating an entry if the key does not exist and overriding an entry if it does.
        /// </summary>
        /// <typeparam name="K">The dictionary key type.</typeparam>
        /// <typeparam name="V">The dictionary value type.</typeparam>
        /// <param name="dictionary">The dictionary object to add the value to.</param>
        /// <param name="key">The key to which the value should be added.</param>
        /// <param name="value">The value to add.</param>
        public static void Put<K, V>([NotNull] this Dictionary<K, V> dictionary, [NotNull] K key, [CanBeNull] V value)
        {
            lock (dictionary)
            {
                if (dictionary.ContainsKey(key))
                {
                    try { (dictionary[key] as IDisposable)?.Dispose(); }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch { }
                    dictionary[key] = value;
                }
                else { dictionary.Add(key, value); }
            }
        }

        /// <summary>
        /// Attempts to determine if a TcpClient object is actually connected in a more reliable fashon than alternative methods.
        /// </summary>
        /// <param name="client">The TcpClient object to check for connectivity.</param>
        /// <returns>True if the TcpClient appears to be connected, otherwise false.</returns>
        public static bool IsConnected([CanBeNull] this TcpClient client)
        {
            try
            {
                if (client?.Client != null && client.Client.Connected)
                {
                    if (client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (client.Client.Receive(buff, SocketFlags.Peek) == 0) { return false; }
                        return true;
                    }
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
    }
}
