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
            ["BitbucketAuthenticationOptions:Username"] =
                _envVarProvider.GetString("BITBUCKET_USERNAME"),
            ["BitbucketAuthenticationOptions:AppPassword"] =
                _envVarProvider.GetString("BITBUCKET_APP_PASSWORD"),

            ["PipeOptions:CreateBuildStatus"] =
                _envVarProvider.GetStringOrDefault("CREATE_BUILD_STATUS", "true"),
            ["PipeOptions:InspectionsXmlPathOrPattern"] =
                _envVarProvider.GetString("INSPECTIONS_XML_PATH"),
            ["PipeOptions:IncludeOnlyIssuesInDiff"] =
                _envVarProvider.GetStringOrDefault("INCLUDE_ONLY_ISSUES_IN_DIFF", "false"),
            ["PipeOptions:FailWhenIssuesFound"] =
                _envVarProvider.GetStringOrDefault("FAIL_WHEN_ISSUES_FOUND", "false"),
        };
    }
}
