using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum State
{
    [EnumMember(Value = "SUCCESSFUL")] Successful,

    [EnumMember(Value = "FAILED")] Failed,

    [EnumMember(Value = "INPROGRESS")] InProgress,

    [EnumMember(Value = "STOPPED")] Stopped,
}
