using IO.Milvus.Param;
using System;
using System.Collections.Generic;
using System.Text;

namespace IO.Milvus.Client
{
    public interface IMilvusClient
    {
        /// <summary>
        /// Time setting for rpc call
        /// </summary>
        /// <param name="timeSpan">time span</param>
        /// <returns></returns>
        IMilvusClient WithTimeout(TimeSpan timeSpan);

        /// <summary>
        /// Disconnects from a Milvus server with configurable timeout.
        /// </summary>
        void Close();

        R<bool> HasCollection(HasCollectionParam);
    }
}
