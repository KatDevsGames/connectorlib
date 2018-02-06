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
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace ConnectorLib
{
    /// <summary>
    /// Describes the serialized object required by the LuaConnector LUA script.
    /// </summary>
    internal class LuaBlock : ISerializable
    {
        /// <summary>
        /// The request identifier. This identifier will be identical in any responses.
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// The creation timestamp for this block.
        /// </summary>
        public DateTimeOffset Stamp { get; }

        /// <summary>
        /// The block command type.
        /// </summary>
        public CommandType Type { get; }

        /// <summary>
        /// The block message (if any).
        /// </summary>
        [NotNull]
        public string Message { get; set; } = "";

        /// <summary>
        /// The 24-bit SNES bus address (if any).
        /// </summary>
        public uint Address { get; set; }

        /// <summary>
        /// The operating value, if any.
        /// </summary>
        public uint Value { get; set; }

        /// <summary>
        /// This is the primary LuaBlock constructor for use with user code.
        /// </summary>
        /// <param name="id">The block identifier.</param>
        /// <param name="type">The block command type.</param>
        public LuaBlock(uint id, CommandType type)
        {
            ID = id;
            Type = type;
            Stamp = DateTime.UtcNow;
        }

        /// <summary>
        /// The serialization constructor. This function is normally not called directly by user code.
        /// </summary>
        /// <param name="info">The SerializationInfo object from the serializer.</param>
        /// <param name="context">This parameter is ignored.</param>
        public LuaBlock([NotNull] SerializationInfo info, StreamingContext context)
        {
            ID = info.GetUInt32("id");
            Stamp = DateTimeOffset.FromUnixTimeSeconds(info.GetInt64("stamp"));
            Type = (CommandType)info.GetByte("type");
            Message = info.GetString("message") ?? string.Empty;
            Address = info.GetUInt32("address");
            Value = info.GetUInt32("value");
        }

        /// <summary>
        /// The serialization getter. This function is normally not called directly by user code.
        /// </summary>
        /// <param name="info">The SerializationInfo object from the serializer.</param>
        /// <param name="context">This parameter is ignored.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", ID);
            info.AddValue("stamp", Stamp.ToUnixTimeSeconds());
            info.AddValue("type", (byte)Type);
            info.AddValue("message", Message);
            info.AddValue("address", Address);
            info.AddValue("value", Value);
        }

        /// <summary>
        /// The block command type enumeration.
        /// </summary>
        public enum CommandType : byte
        {
            [Description("Read Byte")]
            ReadByte = 0x00,
            [Description("Read Word")]
            ReadWord = 0x01,
            [Description("Write Byte")]
            WriteByte = 0x10,
            [Description("Write Word")]
            WriteWord = 0x11,
            [Description("Set Bits")]
            SetBits = 0x20,
            [Description("Unset Bits")]
            UnsetBits = 0x21,
            [Description("Send Message")]
            SendMessage = 0xF0,
            [Description("KeepAlive")]
            KeepAlive = 0xFF
        }
    }
}
