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

namespace ConnectorLib
{
    /// <summary>
    /// Provides an interface for SNES connector objects to implement.
    /// </summary>
    public interface INESConnector : IGameConnector
    {
        /// <summary>
        /// Writes a byte to the NES.
        /// </summary>
        /// <param name="address">The 16-bit NES bus address.</param>
        /// <param name="value">The value to be written.</param>
        /// <returns>True if the write was successful, otherwise false.</returns>
        bool WriteByte(ushort address, byte value);

        /// <summary>
        /// Reads a byte from the NES.
        /// </summary>
        /// <param name="address">The 16-bit NES bus address.</param>
        /// <returns>The byte to be read if the read was successful, otherwise null.</returns>
        byte? ReadByte(ushort address);

        /// <summary>
        /// Sets a bitmask at the specified address.
        /// </summary>
        /// <param name="address">The 16-bit NES bus address.</param>
        /// <param name="value">The bit mask.</param>
        /// <returns>True if the write was successful, otheriwse false.</returns>
        bool SetBit(ushort address, byte value);

        /// <summary>
        /// Unsets a bitmask at the specified address.
        /// </summary>
        /// <param name="address">The 16-bit NES bus address.</param>
        /// <param name="value">The bit mask.</param>
        /// <returns>True if the write was successful, otheriwse false.</returns>
        bool UnsetBit(ushort address, byte value);
    }
}