using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;

[XmlRoot(ElementName = "Issue")]
public class Issue
{
    [XmlAttribute(AttributeName = "TypeId")]
    public string TypeId { get; set; } = null!;

    [XmlAttribute(AttributeName = "File")]
    public string File { get; set; } = null!;

    [XmlAttribute(AttributeName = "Offset")]
    public string Offset { get; set; } = null!;

    [XmlAttribute(AttributeName = "Line")]
    public int Line { get; set; }

    [XmlAttribute(AttributeName = "Message")]
    public string Message { get; set; } = null!;
}
