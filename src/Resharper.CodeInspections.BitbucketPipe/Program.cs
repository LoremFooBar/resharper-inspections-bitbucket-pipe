using System.Diagnostics.CodeAnalysis;
using Resharper.CodeInspections.BitbucketPipe.Utils;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe;

[ExcludeFromCodeCoverage]
internal static class Program
{
    // ReSharper disable once InconsistentNaming
    private static async Task<int> Main()
    {
        var environmentVariableProvider = new EnvironmentVariableProvider();
        bool isDebugMode = new PipeEnvironment(environmentVariableProvider).IsDebugMode;
        Log.Logger = LoggerInitializer.CreateLogger(isDebugMode);
        Log.Debug("DEBUG={IsDebug}", isDebugMode);

        var exitCode = await new PipeRunner(environmentVariableProvider).RunPipeAsync();

        return exitCode.Code;
    }
}
