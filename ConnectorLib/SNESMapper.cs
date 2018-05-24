using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorLib
{
    internal class SNESMapper : IMemoryMapper
    {
        private readonly MapperType _mapper_type;

        public enum MapperType { LOROM, HIROM }

        public SNESMapper(MapperType mapperType) { _mapper_type = mapperType; }

        public (uint offset, string domain) Translate(uint offset)
        {
            switch (_mapper_type)
            {
                case MapperType.LOROM:
                {
                    offset &= 0x7fffffu;
                    if (offset <= 0x3FFFFF)
                    {
                        if ((offset & 0xFFFF) <= 0x1FFF)
                        {
                            return (offset & 0xFFFF, "WRAM");
                        }
                        if ((offset & 0xFFFF) < 0x8000)
                        {
                            return (0, "NOMAP");
                        }
                        uint bank = ((offset & 0x10000) >> 2);
                        return ((offset - 0x8000) - (0x8000 * bank), "ROM");
                    }
                    if ((offset >= 0x700000) && (offset <= 0x7DFFFF))
                    {
                        offset -= 0x700000;
                        uint bank = ((offset & 0x10000) >> 2);
                        if ((offset & 0xFFFF) < 0x8000)
                        {
                            return (offset - (0x8000 * bank), "SRAM");
                        }
                        return (((offset - 0x8000) - (0x8000 * bank)) + 0x380000, "ROM");
                    }
                    if (offset >= 0x7e0000u)
                    {
                        return (offset - 0x7e0000u, "WRAM");
                    }
                    return (0, null);
                }
                case MapperType.HIROM:
                default:
                    return (0, null);
            }
        }

        public uint? ToWRAM(uint snesOffset)
        {
            snesOffset &= 0x7fffffu;
            if (snesOffset >= 0x7e0000u) { return snesOffset - 0x7e0000u; }
            return null;
        }
        public uint Translate(uint offset, string domain) { throw new NotImplementedException(); }
    }
}
