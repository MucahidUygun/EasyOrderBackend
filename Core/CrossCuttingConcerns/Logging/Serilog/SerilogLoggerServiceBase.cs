using Core.CrossCuttingConcerns.Logging.Abstraction;
using SeriLog =Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Logging.Serilog;

public abstract class SerilogLoggerServiceBase : ILogger
{
    protected SeriLog.ILogger Logger { get; set; }

    protected SerilogLoggerServiceBase(SeriLog.ILogger logger)
    {
        Logger = logger;
    }

    public void Critical(string message)
    {
        Logger.Fatal(message);
    }

    public void Debug(string message)
    {
        Logger.Debug(message);
    }

    public void Error(string message)
    {
        Logger.Error(message);
    }

    public void Information(string message)
    {
        Logger.Information(message);
    }

    public void Trace(string message)
    {
        Logger.Verbose(message);
    }

    public void Warning(string message)
    {
        Logger.Warning(message);
    }
}
