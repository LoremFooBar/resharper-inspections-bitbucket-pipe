using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[PublicAPI]
public enum Severity
{
    [EnumMember(Value = "CRITICAL")] Critical,

    [EnumMember(Value = "HIGH")] High,

    [EnumMember(Value = "MEDIUM")] Medium,

    [EnumMember(Value = "LOW")] Low,
}
