using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DiffPatch.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
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

            var codeChanges = await _bitbucketClient.GetCodeChangesAsync();
            var filteredIssues = issues.Where(issue => IsIssueInChanges(issue, codeChanges)).ToList();
            return filteredIssues;

            static bool IsIssueInChanges(Issue issue, IReadOnlyDictionary<string, List<ChunkRange>> codeChanges) =>
                codeChanges.ContainsKey(issue.File) &&
                codeChanges[issue.File].Any(range =>
                    issue.Line >= range.StartLine && issue.Line <= range.StartLine + range.LineCount - 1);
        }
    }
}
