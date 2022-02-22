﻿using System.Diagnostics.CodeAnalysis;
using Serilog;
using Serilog.Core;

namespace Resharper.CodeInspections.BitbucketPipe;

public static class LoggerInitializer
{
    [ExcludeFromCodeCoverage]
    public static Logger CreateLogger(bool isDebugOn)
    {
        var loggerConfig = new LoggerConfiguration().WriteTo.Console();

        if (isDebugOn)
            loggerConfig.MinimumLevel.Debug();
        else
            loggerConfig.MinimumLevel.Warning();

        return loggerConfig.CreateLogger();
    }
}
