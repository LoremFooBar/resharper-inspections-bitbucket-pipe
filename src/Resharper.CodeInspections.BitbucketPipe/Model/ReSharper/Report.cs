using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;

[XmlRoot(ElementName = "Report")]
public class Report
{
    [XmlElement(ElementName = "Information")]
    public Information Information { get; set; } = null!;

    [XmlElement(ElementName = "IssueTypes")]
    public IssueTypes IssueTypes { get; set; } = null!;

    [XmlElement(ElementName = "Issues")]
    public Issues Issues { get; set; } = null!;

    [XmlIgnore]
    public List<Issue> AllIssues =>
        Issues.Projects?.Where(p => p.Issues != null).SelectMany(x => x.Issues!).ToList() ?? new List<Issue>();
}
