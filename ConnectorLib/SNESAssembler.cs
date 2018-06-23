using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ConnectorLib
{
    public class SNESAssembler
    {
        public static class Instructions
        {
            [NotNull] public static byte[] LDA_c(byte value) => new byte[] { 0xA9, value };
            [NotNull] public static byte[] LDA_c(ushort value) => new byte[] { 0xA9, (byte)(value & 0xFF), (byte)(value >> 8) };
            [NotNull] public static byte[] LDA_as(ushort addr) => new byte[] { 0xAD, (byte)(addr & 0xFF), (byte)(addr >> 8) };
            [NotNull] public static byte[] STA_as(ushort addr) => new byte[] { 0x8D, (byte)(addr & 0xFF), (byte)(addr >> 8) };
            [NotNull] public static byte[] STZ_as(ushort addr) => new byte[] { 0x9C, (byte)(addr & 0xFF), (byte)(addr >> 8) };
        }
    }
}
