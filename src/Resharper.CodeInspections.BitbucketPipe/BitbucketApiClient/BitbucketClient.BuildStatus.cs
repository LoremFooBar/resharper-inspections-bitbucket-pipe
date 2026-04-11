using Microsoft.Extensions.Logging;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;

namespace Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;

public partial class BitbucketClient
{
    public async Task CreateBuildStatusAsync(PipelineReport report)
    {
        if (!_pipeOptions.CreateBuildStatus) return;

        var buildStatus = BuildStatus.CreateFromPipelineReport(report, _bitbucketEnvironmentInfo.Workspace,
            _bitbucketEnvironmentInfo.RepoSlug);
        string serializedBuildStatus = Serialize(buildStatus);

        _logger.LogDebug("POSTing build status: {BuildStatus}", serializedBuildStatus);

        var response = await _httpClient.PostAsync($"commit/{_bitbucketEnvironmentInfo.CommitHash}/statuses/build",
            CreateStringContent(serializedBuildStatus));

        await VerifyResponseAsync(response);
    }
}
