using System.IO;

namespace Resharper.CodeInspections.BitbucketPipe.Options
{
    public class PipeOptions
    {
        /// <summary>
        /// Whether to create a build status representing whether any issues were found.
        /// </summary>
        public bool CreateBuildStatus { get; set; }

        /// <summary>
        /// Path to inspections XML report. Can use patterns that are supported by <see cref="DirectoryInfo.GetFiles()"/>
        /// </summary>
        public string InspectionsXmlPathOrPattern { get; set; } = "";

        /// <summary>
        /// Whether to fail only for issues found in diff. For PRs - the PR diff, otherwise - diff with previous commit.
        /// </summary>
        public bool IncludeOnlyIssuesInDiff { get; set; }
    }
}
