using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum AnnotationType
    {
        [EnumMember(Value = "VULNERABILITY")]
        Vulnerability,

        [EnumMember(Value = "CODE_SMELL")]
        CodeSmell,

        [EnumMember(Value = "BUG")]
        Bug
    }
}
