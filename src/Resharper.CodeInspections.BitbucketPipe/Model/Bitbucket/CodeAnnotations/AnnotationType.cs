using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[PublicAPI]
public enum AnnotationType
{
    [EnumMember(Value = "VULNERABILITY")] Vulnerability,

    [EnumMember(Value = "CODE_SMELL")] CodeSmell,

    [EnumMember(Value = "BUG")] Bug,
}
