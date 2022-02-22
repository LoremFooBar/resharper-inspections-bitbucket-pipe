using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[PublicAPI]
public enum State
{
    [EnumMember(Value = "SUCCESSFUL")] Successful,

    [EnumMember(Value = "FAILED")] Failed,

    [EnumMember(Value = "INPROGRESS")] InProgress,

    [EnumMember(Value = "STOPPED")] Stopped,
}
