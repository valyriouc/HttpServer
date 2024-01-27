namespace Server.Core.Logging;

public enum LogState
{
    Info,
    Warn, 
    Error
}

public static class LogStateExtensions
{
    public static string WriteAs(this LogState state) => state switch
    {
        LogState.Info => "INFO",
        LogState.Warn => "WARN",
        LogState.Error => "ERROR",
        _ => throw new NotImplementedException("Conversion for this log state is not implemented!")
    };
}

public interface ILogger
{
    public void Info(string message);

    public void Warn(string message);

    public void Error(Exception ex);
}
