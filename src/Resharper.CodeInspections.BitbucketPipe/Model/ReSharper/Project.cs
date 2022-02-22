using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper
{
    [XmlRoot(ElementName = "Project")]
    public class Project
    {
        [XmlElement(ElementName = "Issue")]
        public List<Issue>? Issues { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string? Name { get; set; }
    }
}
