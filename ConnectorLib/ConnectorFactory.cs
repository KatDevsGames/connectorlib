using System;
using JetBrains.Annotations;

namespace ConnectorLib
{
    public static class ConnectorFactory
    {
        [NotNull]
        public static Type GetConnectorType(ConnectorType type)
        {
            switch (type)
            {
                case ConnectorType.SNESConnector:
                    return typeof(ISNESConnector);
                case ConnectorType.NESConnector:
                    return typeof(INESConnector);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
