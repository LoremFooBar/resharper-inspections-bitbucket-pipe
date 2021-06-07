using System.IO;

namespace Resharper.CodeInspections.BitbucketPipe.Tests
{
    public static class TestUtils
    {
        private const string ExampleReportsDirectoryName = "example-reports";

        public static string GetNonEmptyReportFilePath() =>
            Path.Combine(ExampleReportsDirectoryName, "report-with-issues.xml");

        public static string GetEmptyReportFilePath() =>
            Path.Combine(ExampleReportsDirectoryName, "report-without-issues.xml");
    }
}
