namespace Resharper.CodeInspections.BitbucketPipe;

public class RequiredEnvironmentVariableNotFoundException : Exception
{
    public RequiredEnvironmentVariableNotFoundException(string variableName) :
        base($"Required environment variable {variableName} not found") { }
}
