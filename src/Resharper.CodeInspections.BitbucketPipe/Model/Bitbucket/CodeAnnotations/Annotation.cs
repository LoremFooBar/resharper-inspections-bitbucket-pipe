using JetBrains.Annotations;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;

[Serializable]
[PublicAPI]
public class Annotation
{
    public string? ExternalId { get; set; }
    public string? Uuid { get; set; }
    public AnnotationType AnnotationType { get; set; }
    public string Path { get; set; } = "";
    public int Line { get; set; }
    public string? Summary { get; set; }
    public string? Details { get; set; }
    public AnnotationResult Result { get; set; }
    public Severity? Severity { get; set; }
    public Uri? Link { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
