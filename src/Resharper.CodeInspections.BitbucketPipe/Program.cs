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
            Log.Logger = LoggerInitializer.CreateLogger(EnvironmentUtils.IsDebugMode);
            Log.Debug("DEBUG={IsDebug}", EnvironmentUtils.IsDebugMode);

            await new PipeRunner().RunPipeAsync();
        }
    }
}
