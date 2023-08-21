namespace Resharper.CodeInspections.BitbucketPipe.Options;

public class PipeOptions
{
    public const string SectionName = "PipeOptions";

    /// <summary>
    /// Whether to create a build status representing whether any issues were found.
    /// </summary>
    public bool CreateBuildStatus { get; set; }

    /// <summary>
    /// Path to inspections XML report. Can use patterns that are supported by <see cref="DirectoryInfo.GetFiles()" />
    /// </summary>
    public string InspectionsXmlPathOrPattern { get; set; } = "";

    /// <summary>
    /// Whether to fail only for issues found in diff. For PRs - the PR diff, otherwise - diff with previous commit.
    /// </summary>
    public bool IncludeOnlyIssuesInDiff { get; set; }

    /// <summary>
    /// Whether to fail current build step if any issues found.
    /// </summary>
    public bool FailWhenIssuesFound { get; set; }
}
