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
using JetBrains.Annotations;

namespace ConnectorLib
{
    public interface IBatchWriteContext : IDisposable
    {
    }

    /// <summary>
    /// Provides an interface for SNES connector objects to implement.
    /// </summary>
    public interface ISNESConnector : IDisposable
    {
        /// <summary>
        /// Opens a context which can be used to perform multiple writes in a synchronized fashion. This should be used anytime
        /// multiple writes are done "at the same time", as some supported platforms may have limits on the frequency of supported
        /// write operations.
        /// </summary>
        /// <returns>A context for performing batch writes.</returns>
        IBatchWriteContext OpenBatchWriteContext();

        /// <summary>
        /// Writes a byte to the SNES.
        /// </summary>
        /// <param name="address">The 24-bit SNES bus address.</param>
        /// <param name="value">The value to be written.</param>
        /// <returns>True if the write was successful, otherwise false.</returns>
        bool WriteByte(uint address, byte value);

        /// <summary>
        /// Writes an usigned short to the SNES.
        /// </summary>
        /// <param name="address">The 24-bit SNES bus address.</param>
        /// <param name="value">The value to be written.</param>
        /// <returns>True if the write was successful, otherwise false.</returns>
        bool WriteWord(uint address, ushort value);

        /// <summary>
        /// Reads a byte from the SNES.
        /// </summary>
        /// <param name="address">The 24-bit SNES bus address.</param>
        /// <returns>The byte to be read if the read was successful, otherwise null.</returns>
        byte? ReadByte(uint address);

        /// <summary>
        /// Reads an unsigned short from the SNES.
        /// </summary>
        /// <param name="address">The 24-bit SNES bus address.</param>
        /// <returns>The byte to be read if the read was successful, otherwise null.</returns>
        ushort? ReadWord(uint address);

        /// <summary>
        /// Sends a message to be displayed.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <returns>True if the text display was successful, otherwise false.</returns>
        /// <remarks>Connector implementations that do not support messaging should return false rather than throwing a NotSupportedException.</remarks>
        bool SendMessage([CanBeNull] string message);

        /// <summary>
        /// Sets a bitmask at the specified address.
        /// </summary>
        /// <param name="address">The 24-bit SNES bus address.</param>
        /// <param name="value">The bit mask.</param>
        /// <returns>True if the write was successful, otheriwse false.</returns>
        bool SetBit(uint address, byte value);

        /// <summary>
        /// Unsets a bitmask at the specified address.
        /// </summary>
        /// <param name="address">The 24-bit SNES bus address.</param>
        /// <param name="value">The bit mask.</param>
        /// <returns>True if the write was successful, otheriwse false.</returns>
        bool UnsetBit(uint address, byte value);

        /// <summary>
        /// True if the connector object is connected to the game, otherwise false.
        /// </summary>
        bool Connected { get; }
    }
}