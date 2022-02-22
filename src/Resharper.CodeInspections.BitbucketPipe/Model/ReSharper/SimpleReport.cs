namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;

public record SimpleReport(string Solution, IReadOnlyList<IssueType> IssueTypes, IReadOnlyList<Issue> Issues)
{
    public int TotalIssues => Issues.Count;

    public bool HasAnyIssues => Issues.Any();
}
