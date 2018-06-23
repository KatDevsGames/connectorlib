using System;
using JetBrains.Annotations;

namespace ConnectorLib
{
    public interface IGameConnector : IDisposable
    {
        /// <summary>
        /// Opens a context which can be used to perform multiple writes in a synchronized fashion. This should be used anytime
        /// multiple writes are done "at the same time", as some supported platforms may have limits on the frequency of supported
        /// write operations.
        /// </summary>
        /// <returns>A context for performing batch writes.</returns>
        IBatchWriteContext OpenBatchWriteContext();

        /// <summary>
        /// Sends a message to be displayed.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <returns>True if the text display was successful, otherwise false.</returns>
        /// <remarks>Connector implementations that do not support messaging should return false rather than throwing a NotSupportedException.</remarks>
        bool SendMessage([CanBeNull] string message);

        /// <summary>
        /// True if the connector object is connected to the game, otherwise false.
        /// </summary>
        bool Connected { get; }

        bool WriteBytes(uint address, [CanBeNull] byte[] data);
        bool ReadBytes(uint address, [CanBeNull] byte[] buffer);
    }
}
