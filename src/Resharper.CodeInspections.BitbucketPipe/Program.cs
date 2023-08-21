using System.Diagnostics.CodeAnalysis;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task<int> Main()
        => await new PipeRunner(new EnvironmentVariableProvider()).RunPipeAsync();
}
