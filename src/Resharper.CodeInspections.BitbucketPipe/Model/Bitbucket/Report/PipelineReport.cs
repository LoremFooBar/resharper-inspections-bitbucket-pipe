﻿using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;

[Serializable]
[PublicAPI]
public class PipelineReport
{
    //public string Type => "report";
    public string? Uuid { get; set; }
    public string? Title { get; set; }
    public string? Details { get; set; }
    public string? ExternalId { get; set; }
    public string? Reporter { get; set; }
    public Uri? Link { get; set; }
    public ReportType ReportType { get; set; }
    public Result Result { get; set; }
    public ICollection<ReportDataItem> Data { get; set; } = new List<ReportDataItem>();
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    [JsonIgnore]
    public int TotalIssues { get; set; }

    public static PipelineReport CreateFromIssuesReport(SimpleReport issuesReport)
    {
        var pipelineReport = new PipelineReport
        {
            Title = "ReSharper Inspections",
            Details = PipeUtils.GetFoundIssuesString(issuesReport.TotalIssues, issuesReport.Solution),
            ExternalId = "resharper-inspections",
            Reporter = "ReSharper",
            ReportType = ReportType.Bug,
            Result = issuesReport.HasAnyIssues ? Result.Failed : Result.Passed,
            TotalIssues = issuesReport.TotalIssues,
        };

        return pipelineReport;
    }
}
