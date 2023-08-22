namespace Resharper.CodeInspections.BitbucketPipe.Utils;

public interface IEnvironmentVariableProvider
{
    string? GetString(string variableName);

    string GetRequiredString(string variableName) =>
        GetString(variableName) ??
        throw new RequiredEnvironmentVariableNotFoundException(variableName);

    string GetStringOrDefault(string variableName, string defaultValue) =>
        GetString(variableName) ?? defaultValue;
}
