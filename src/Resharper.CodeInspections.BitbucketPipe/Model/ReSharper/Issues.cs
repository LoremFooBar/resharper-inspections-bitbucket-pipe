using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;

[XmlRoot(ElementName = "Issues")]
public class Issues
{
    [XmlElement(ElementName = "Project")]
    public List<Project>? Projects { get; set; }
}
