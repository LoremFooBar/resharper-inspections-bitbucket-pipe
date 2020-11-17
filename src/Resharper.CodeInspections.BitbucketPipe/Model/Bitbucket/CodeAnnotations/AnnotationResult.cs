using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum AnnotationResult
    {
        [EnumMember(Value = "PASSED")]
        Passed,

        [EnumMember(Value = "FAILED")]
        Failed,

        [EnumMember(Value = "SKIPPED")]
        Skipped,

        [EnumMember(Value = "IGNORED")]
        Ignored
    }
}
