using System;
using System.Collections.Generic;

namespace ConnectorLib
{
    public class BusMapper : IMemoryMapper
    {
        public (uint offset, string domain) Translate(uint offset) => (offset, "System Bus");

        public uint Translate(uint offset, string domain)
        {
            if(!string.Equals(domain,"System Bus")) { throw new NotSupportedException("Domain type not supported."); }
            return offset;
        }

        public IEnumerable<string> Domains => new[] { "System Bus" };
    }
}
