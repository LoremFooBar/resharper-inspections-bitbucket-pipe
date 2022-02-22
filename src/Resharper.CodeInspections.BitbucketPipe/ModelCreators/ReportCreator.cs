using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Model.Diff;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Options;

namespace Resharper.CodeInspections.BitbucketPipe.ModelCreators
{
    public class ReSharperReportCreator
    {
        private readonly PipeOptions _options;
        private readonly BitbucketClient _bitbucketClient;
        private readonly ILogger<ReSharperReportCreator> _logger;

        public ReSharperReportCreator(IOptions<PipeOptions> options, BitbucketClient bitbucketClient,
            ILogger<ReSharperReportCreator> logger)
        {
            _options = options.Value;
            _bitbucketClient = bitbucketClient;
            _logger = logger;
        }

        public async Task<SimpleReport> CreateFromFileAsync(string filePathOrPattern)
        {
            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            _logger.LogDebug("Current directory: {Directory}", currentDir);

            var reportFile = currentDir.GetFiles(filePathOrPattern).FirstOrDefault();
            if (reportFile == null) {
                throw new FileNotFoundException($"could not find report file in directory {currentDir.FullName}",
                    filePathOrPattern);
            }

            _logger.LogDebug("Found inspections file: {File}", reportFile.FullName);

            await using var fileStream = reportFile.OpenRead();
            _logger.LogDebug("Deserializing report...");
            object deserializedReport = new XmlSerializer(typeof(Report)).Deserialize(fileStream) ??
                                        throw new SerializationException("Failed to deserialize the report xml");
            _logger.LogDebug("Report deserialized successfully");

            var report = (Report) deserializedReport;


            var normalizedIssues = GetNormalizedIssues(report.AllIssues);
            var filteredIssues = await FilterIssuesByDiffAsync(normalizedIssues);

            var simpleReport = new SimpleReport(report.Information.Solution,
                report.IssueTypes.Types?.AsReadOnly() ?? new List<IssueType>().AsReadOnly(),
                filteredIssues);

            return simpleReport;
        }

        private static IReadOnlyList<Issue> GetNormalizedIssues(IEnumerable<Issue> issues)
        {
            var normalizedIssues = new List<Issue>();

            foreach (var issue in issues) {
                // normalize file path
                issue.File = issue.File.Replace('\\', '/');

                // add issue line when missing (it is not in report xml when it's the first line)
                issue.Line = issue.Line == default ? 1 : issue.Line;

                normalizedIssues.Add(issue);
            }

            return normalizedIssues;
        }

        private async Task<IReadOnlyList<Issue>> FilterIssuesByDiffAsync(IReadOnlyList<Issue> issues)
        {
            if (!_options.IncludeOnlyIssuesInDiff || !issues.Any()) {
                return issues;
            }

            _logger.LogDebug("filtering issues by changes in PR/commit. Total issues: {TotalIssues}", issues.Count);

            var codeChanges = await _bitbucketClient.GetCodeChangesAsync();
            var filteredIssues = issues.Where(issue => IsIssueInChanges(issue, codeChanges)).ToList();

            _logger.LogDebug("Total issues after filter: {TotalFilteredIssues}", filteredIssues.Count);

            return filteredIssues;

            static bool IsIssueInChanges(Issue issue, IReadOnlyDictionary<string, AddedLinesInFile> codeChanges) =>
                codeChanges.ContainsKey(issue.File) &&
                codeChanges[issue.File].LinesAdded.Any(addedLineNum => issue.Line == addedLineNum);
        }
    }
}
