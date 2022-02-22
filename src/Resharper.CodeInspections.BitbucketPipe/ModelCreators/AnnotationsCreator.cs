using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Utils;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe.ModelCreators
{
    public class AnnotationsCreator
    {
        private readonly BitbucketEnvironmentInfo? _environmentInfo;

        public AnnotationsCreator(BitbucketEnvironmentInfo? environmentInfo = null) => _environmentInfo = environmentInfo;

        public IEnumerable<Annotation> CreateAnnotationsFromIssuesReport(SimpleReport report)
        {
            if (!report.HasAnyIssues) {
                yield break;
            }

            var issueTypes = report.IssueTypes.ToDictionary(t => t.Id, t => t);

            for (int i = 0; i < report.TotalIssues; i++) {
                var issue = report.Issues[i];
                var issueType = issueTypes[issue.TypeId];
                string details = issue.Message + (string.IsNullOrWhiteSpace(issueType.WikiUrl)
                    ? ""
                    : Environment.NewLine + $"Wiki URL: {issueType.WikiUrl}");

                string relativePathPart = GetRelativePathPart();

                // // path char in ReSharper report is always '\' regardless of platform
                // string issueFilePath = issue.File.Replace('\\', '/');

                yield return new Annotation
                {
                    ExternalId = $"issue-{i + 1}",
                    AnnotationType = AnnotationType.CodeSmell,
                    Path = Path.Combine(relativePathPart, issue.File),
                    Line = issue.Line,
                    Summary = issueType.Description,
                    Details = details,
                    Result = AnnotationResult.Failed
                };
            }
        }

        private string GetRelativePathPart()
        {
            string currentDir = Environment.CurrentDirectory;
            string clonePath = _environmentInfo?.CloneDir ?? currentDir;

            // get the relative path from clone root dir to current dir
            string relativePathPart = Path.GetRelativePath(clonePath, currentDir);

            if (relativePathPart[0] == '.') {
                relativePathPart = relativePathPart.Remove(0, 1);
            }

            Log.Logger.Debug("clone directory: {ClonePath}", clonePath);
            Log.Logger.Debug("relative path part: {RelativePath}", relativePathPart);

            return relativePathPart;
        }
    }
}
