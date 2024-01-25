using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ServerConsole.Logging;

internal enum LogState
{
    Info,
    Warn, 
    Error
}

internal static class LogStateExtensions
{
    public static string WriteAs(this LogState state) => state switch
    {
        LogState.Info => "INFO",
        LogState.Warn => "WARN",
        LogState.Error => "ERROR",
        _ => throw new NotImplementedException("Conversion for this log state is not implemented!")
    };
}

internal interface ILogger
{
    public void Info(string message);

    public void Warn(string message);

    public void Error(Exception ex);
}
