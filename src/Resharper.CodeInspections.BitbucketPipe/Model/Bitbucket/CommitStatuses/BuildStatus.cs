using System;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses
{
    [Serializable, PublicAPI]
    public class BuildStatus
    {
        public BuildStatus(string key, string name, State state, string description, string workspace, string repoSlug)
        {
            Key = key;
            Name = name;
            State = state;
            Description = description;
            Url = new Uri($"https://bitbucket.org/{workspace}/{repoSlug}");
        }

        public string Key { get; set; }
        public string Name { get; set; }
        public State State { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }

        // ReSharper disable once StringLiteralTypo
        [JsonPropertyName("refname")]
        public string? RefName { get; set; }

        public static BuildStatus CreateFromPipelineReport(PipelineReport report, string workspace, string repoSlug)
        {
            const string key = "ReSharper-Inspections";
            const string name = "ReSharper Inspections";
            var state = report.Result switch
            {
                Result.Failed => State.Failed,
                Result.Passed => State.Successful,
                Result.Pending => throw new ArgumentOutOfRangeException(nameof(report.Result)),
                _ => throw new ArgumentOutOfRangeException(nameof(report.Result))
            };

            string description = Utils.GetFoundIssuesString(report.TotalIssues);

            return new BuildStatus(key, name, state, description, workspace, repoSlug);
        }
    }
}
