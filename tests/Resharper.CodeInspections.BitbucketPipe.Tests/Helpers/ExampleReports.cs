using System.IO;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.Helpers
{
    public static class ExampleReports
    {
        private const string ExampleReportsDirectoryName = "example-reports";

        public static string GetNonEmptyReportFilePath() =>
            Path.Combine(ExampleReportsDirectoryName, "report-with-issues.xml");

        public static string GetEmptyReportFilePath() =>
            Path.Combine(ExampleReportsDirectoryName, "report-without-issues.xml");
    }
}
