using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorLib
{
    public interface IMemoryMapper
    {
        (uint offset, string domain) Translate(uint offset);
        uint Translate(uint offset, string domain);
    }
}
