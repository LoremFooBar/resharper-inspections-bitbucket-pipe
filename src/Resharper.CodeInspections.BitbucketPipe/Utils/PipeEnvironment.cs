namespace Resharper.CodeInspections.BitbucketPipe.Utils;

public class PipeEnvironment
{
    public PipeEnvironment(IEnvironmentVariableProvider environmentVariableProvider)
    {
        IsDebugMode = environmentVariableProvider.GetStringOrDefault("DEBUG", "false")
            .Equals("true", StringComparison.OrdinalIgnoreCase);
        string environmentName =
            environmentVariableProvider.GetStringOrDefault("NETCORE_ENVIRONMENT", "Production");
        IsDevelopment = environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsDebugMode { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IsDevelopment { get; }
}
