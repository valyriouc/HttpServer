using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ServerConsole.Logging;

internal class LogOntoConsole : ILogger
{
    private readonly bool colorize;

    public LogOntoConsole(bool colorize)
    {
        this.colorize = colorize;
    }

    private void MaybeWithColor(ConsoleColor color, LogState state)
    {
        if (this.colorize)
        {
            Console.ForegroundColor = color;
        }

        Console.Write($"{state.WriteAs()}: ");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void Error(Exception ex)
    {
        MaybeWithColor(ConsoleColor.Red, LogState.Error);
        Console.WriteLine(ex);
    }

    public void Info(string message)
    {
        MaybeWithColor(ConsoleColor.Blue, LogState.Info);
        Console.WriteLine(message);
    }

    public void Warn(string message)
    {
        MaybeWithColor(ConsoleColor.Yellow, LogState.Warn);
        Console.WriteLine(message);
    }
}
