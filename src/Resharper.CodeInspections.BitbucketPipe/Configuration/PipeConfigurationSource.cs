using Microsoft.Extensions.Configuration;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Configuration;

public class PipeConfigurationSource : IConfigurationSource
{
    private readonly IEnvironmentVariableProvider _envVarProvider;

    public PipeConfigurationSource(IEnvironmentVariableProvider envVarProvider) => _envVarProvider = envVarProvider;

    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new PipeConfigurationProvider(_envVarProvider);
}
