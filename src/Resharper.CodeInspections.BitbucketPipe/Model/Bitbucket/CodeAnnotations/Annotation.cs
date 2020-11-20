using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations
{
    [Serializable]
    public class Annotation
    {
        public string? ExternalId { get; set; }
        public string? Uuid { get; set; }
        public AnnotationType AnnotationType { get; set; }
        public string Path { get; set; } = "";
        public int Line { get; set; }
        public string? Summary { get; set; }
        public string? Details { get; set; }
        public AnnotationResult Result { get; set; }
        public Severity? Severity { get; set; }
        public Uri? Link { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public static IEnumerable<Annotation> CreateFromIssuesReport(ReSharper.Report report)
        {
            if (!report.HasAnyIssues) {
                yield break;
            }

            var issueTypes = report.IssueTypes.Types!.ToDictionary(t => t.Id, t => t);

            for (int i = 0; i < report.TotalIssues; i++) {
                var issue = report.AllIssues[i];
                var issueType = issueTypes[issue.TypeId];
                string details = issue.Message + (string.IsNullOrWhiteSpace(issueType.WikiUrl)
                    ? ""
                    : Environment.NewLine + $"Wiki URL: {issueType.WikiUrl}");

                string relativePathPart = GetRelativePathPart();

                // path char in ReSharper report is always '\' regardless of platform
                string issueFilePath = issue.File.Replace('\\', '/');

                yield return new Annotation
                {
                    ExternalId = $"issue-{i + 1}",
                    AnnotationType = AnnotationType.CodeSmell,
                    Path = System.IO.Path.Combine(relativePathPart, issueFilePath),
                    Line = issue.Line,
                    Summary = issueType.Description,
                    Details = details,
                    Result = AnnotationResult.Failed
                };
            }
        }

        private static string GetRelativePathPart()
        {
            string currentDir = Environment.CurrentDirectory;
            string clonePath = Environment.GetEnvironmentVariable("BITBUCKET_CLONE_DIR") ?? currentDir;

            // get the relative path from clone root dir to current dir
            string relativePathPart = System.IO.Path.GetRelativePath(clonePath, currentDir);

            if (relativePathPart[0] == '.') {
                relativePathPart = relativePathPart.Remove(0, 1);
            }

            Log.Logger.Debug("clone directory: {clonePath}", clonePath);
            Log.Logger.Debug("relative path part: {relativePath}", relativePathPart);

            return relativePathPart;
        }
    }
}
