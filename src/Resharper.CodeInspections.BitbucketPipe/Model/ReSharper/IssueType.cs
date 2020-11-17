using System.Xml.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper
{
    [XmlRoot(ElementName = "IssueType")]
    public class IssueType
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; } = null!;

        [XmlAttribute(AttributeName = "Category")]
        public string Category { get; set; } = null!;

        [XmlAttribute(AttributeName = "CategoryId")]
        public string CategoryId { get; set; } = null!;

        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; } = null!;

        [XmlAttribute(AttributeName = "Severity")]
        public string Severity { get; set; } = null!;

        [XmlAttribute(AttributeName = "WikiUrl")]
        public string? WikiUrl { get; set; }

        // [XmlAttribute(AttributeName = "SubCategory")]
        // public string? SubCategory { get; set; }

        // [XmlAttribute(AttributeName = "Global")]
        // public string? Global { get; set; }
    }
}
