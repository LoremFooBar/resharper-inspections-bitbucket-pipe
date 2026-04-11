using Microsoft.Extensions.Configuration;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Configuration;

public class PipeConfigurationProvider : ConfigurationProvider
{
    private readonly IEnvironmentVariableProvider _envVarProvider;

    public PipeConfigurationProvider(IEnvironmentVariableProvider envVarProvider) =>
        _envVarProvider = envVarProvider;

    public override void Load()
    {
        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["BitbucketAuthenticationOptions:Email"] =
                _envVarProvider.GetRequiredString("ACCOUNT_EMAIL"),
            ["BitbucketAuthenticationOptions:ApiToken"] =
                _envVarProvider.GetRequiredString("API_TOKEN"),

            ["PipeOptions:CreateBuildStatus"] =
                _envVarProvider.GetStringOrDefault("CREATE_BUILD_STATUS", "true"),
            ["PipeOptions:InspectionsXmlPathOrPattern"] =
                _envVarProvider.GetRequiredString("INSPECTIONS_XML_PATH"),
            ["PipeOptions:IncludeOnlyIssuesInDiff"] =
                _envVarProvider.GetStringOrDefault("INCLUDE_ONLY_ISSUES_IN_DIFF", "false"),
            ["PipeOptions:FailWhenIssuesFound"] =
                _envVarProvider.GetStringOrDefault("FAIL_WHEN_ISSUES_FOUND", "false"),
        };
    }
}
