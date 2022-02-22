using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;

[XmlRoot(ElementName = "IssueTypes")]
public class IssueTypes
{
    [XmlElement(ElementName = "IssueType")]
    public List<IssueType>? Types { get; set; }
}
