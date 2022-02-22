using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;

[XmlRoot(ElementName = "Information")]
public class Information
{
    [XmlElement(ElementName = "Solution")]
    public string Solution { get; set; } = null!;

    // [XmlElement(ElementName = "InspectionScope")]
    // public InspectionScope InspectionScope { get; set; }
}
