using System.Collections.Generic;
using JetBrains.Annotations;

namespace ConnectorLib
{
    public interface IMemoryMapper
    {
        (uint offset, string domain) Translate(uint offset);
        uint Translate(uint offset, string domain);
        [NotNull] IEnumerable<string> Domains { get; }
    }
}
