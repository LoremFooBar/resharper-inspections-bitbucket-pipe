using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum Severity
{
    [EnumMember(Value = "CRITICAL")] Critical,

    [EnumMember(Value = "HIGH")] High,

    [EnumMember(Value = "MEDIUM")] Medium,

    [EnumMember(Value = "LOW")] Low,
}
