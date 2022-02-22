namespace Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

public static class TestData
{
    private const string testDataDirectoryName = "test-data";

    public static string NonEmptyReportFilePath { get; } =
        Path.Combine(testDataDirectoryName, "report-with-issues.xml");

    public static string EmptyReportFilePath { get; } =
        Path.Combine(testDataDirectoryName, "report-without-issues.xml");

    public static string NonEmptyReportForDiffFilePath { get; } =
        Path.Combine(testDataDirectoryName, "report-with-issues-for-diff.xml");

    /// <summary>
    /// this diff should produce changes
    /// in file
    /// tests/Resharper.CodeInspections.BitbucketPipe.Tests/ResharperReportTests/When_Creating_Report_From_File_That_Does_Not_Contain_Issues.cs
    /// in lines 1-6
    /// </summary>
    public static string DiffText { get; } =
        File.ReadAllText(Path.Combine(testDataDirectoryName, "diff.txt")).Replace("\r\n", "\n");
}
