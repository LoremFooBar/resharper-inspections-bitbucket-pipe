using System.Diagnostics.CodeAnalysis;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;

[Serializable]
[ExcludeFromCodeCoverage] // not used by this pipe
public class ReportDataItem
{
    public ReportDataType Type { get; set; }
    public string Title { get; set; } = null!;
    public object Value { get; set; } = null!;
}
