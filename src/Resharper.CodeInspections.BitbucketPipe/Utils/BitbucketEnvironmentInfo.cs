using System.Diagnostics.CodeAnalysis;

namespace Resharper.CodeInspections.BitbucketPipe.Utils;

public class BitbucketEnvironmentInfo
{
    public BitbucketEnvironmentInfo(IEnvironmentVariableProvider? environmentVariableProvider = null)
    {
        if (environmentVariableProvider == null) return;

        CommitHash = environmentVariableProvider.GetRequiredString("BITBUCKET_COMMIT");
        Workspace = environmentVariableProvider.GetRequiredString("BITBUCKET_WORKSPACE");
        RepoSlug = environmentVariableProvider.GetRequiredString("BITBUCKET_REPO_SLUG");
        CloneDir = environmentVariableProvider.GetRequiredString("BITBUCKET_CLONE_DIR");

        PullRequestId = environmentVariableProvider.GetString("BITBUCKET_PR_ID");
        IsPullRequest = !string.IsNullOrEmpty(PullRequestId);
    }

    public string Workspace { get; init; } = "";

    public string RepoSlug { get; init; } = "";

    public string CommitHash { get; init; } = "";

    public string? CloneDir { get; }

    public string? PullRequestId { get; init; }

    [MemberNotNullWhen(true, nameof(PullRequestId))]
    public bool IsPullRequest { get; }
}
