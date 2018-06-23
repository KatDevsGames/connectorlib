using System.Collections.Generic;

namespace ConnectorLib
{
    public class sd2snesBatchWriteContext : IBatchWriteContext
    {
        sd2snesConnector mOwner;

        public sd2snesBatchWriteContext(sd2snesConnector owner)
        {
            mOwner = owner;
        }

        public List<sd2snesConnector.WriteDescriptor> BatchWriteEntries = new List<sd2snesConnector.WriteDescriptor>();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Perform writes here
                    mOwner.TransmitBatchWrites();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
