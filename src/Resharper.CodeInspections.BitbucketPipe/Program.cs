using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Resharper.CodeInspections.BitbucketPipe.Utils;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        // ReSharper disable once InconsistentNaming
        private static async Task Main()
        {
            bool isDebugMode = new PipeEnvironment(new EnvironmentVariableProvider()).IsDebugMode;
            Log.Logger = LoggerInitializer.CreateLogger(isDebugMode);
            Log.Debug("DEBUG={IsDebug}", isDebugMode);

            await new PipeRunner(new EnvironmentVariableProvider()).RunPipeAsync();
        }
    }
}
