namespace Resharper.CodeInspections.BitbucketPipe.Utils;

public class EnvironmentVariableProvider : IEnvironmentVariableProvider
{
    public string? GetString(string variableName) => Environment.GetEnvironmentVariable(variableName);
}
