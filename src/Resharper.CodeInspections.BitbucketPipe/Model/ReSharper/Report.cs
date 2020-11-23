using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper
{
    [XmlRoot(ElementName = "Report")]
    public class Report
    {
        [XmlElement(ElementName = "Information")]
        public Information Information { get; set; } = null!;

        [XmlElement(ElementName = "IssueTypes")]
        public IssueTypes IssueTypes { get; set; } = null!;

        [XmlElement(ElementName = "Issues")]
        public Issues Issues { get; set; } = null!;

        // [XmlAttribute(AttributeName = "ToolsVersion")]
        // public string ToolsVersion { get; set; }

        [XmlIgnore]
        public int TotalIssues => AllIssues.Count;

        [XmlIgnore]
        public bool HasAnyIssues => Issues.Projects?.Any() == true;

        [XmlIgnore]
        public List<Issue> AllIssues => Issues.Projects?.SelectMany(x => x.Issues).ToList() ?? new List<Issue>();
    }
}
