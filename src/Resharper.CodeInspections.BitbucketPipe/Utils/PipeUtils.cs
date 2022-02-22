namespace Resharper.CodeInspections.BitbucketPipe.Utils;

internal static class PipeUtils
{
    public static string GetFoundIssuesString(int numOfIssues) =>
        numOfIssues > 0 ? $"Found {numOfIssues} issue(s)" : "No issues found";

    public static string GetFoundIssuesString(int numOfIssues, string solutionName) =>
        $"{GetFoundIssuesString(numOfIssues)} in solution {solutionName}";
}
