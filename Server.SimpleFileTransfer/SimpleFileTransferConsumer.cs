using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectorize.Logging;
using Vectorize.Server.Handles;
using Vectorize.Server.Server;

namespace Server.SimpleFileTransfer;

internal class SimpleFileTransferConsumer : IServerConsumer
{
    public ILogger Logger {get;set;}

    public SimpleFileTransferConsumer(ILogger logger)
    {
        Logger = logger;
    }

    public async Task ConsumeAsync(IByteHandle handle, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
