using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipelineReportTests
{
    public class When_Creating_Pipeline_Report_From_Report_That_Contains_Issues : SpecificationBase
    {
        private PipelineReport _pipelineReport;

        protected override async Task WhenAsync()
        {
            var report = await Report.CreateFromFileAsync(ExampleReports.GetNonEmptyReportFilePath());
            _pipelineReport = PipelineReport.CreateFromIssuesReport(report);
        }

        [Then]
        public void It_Should_Create_Pipeline_Report_That_Contains_Issues()
        {
            _pipelineReport.TotalIssues.Should().Be(4);
            _pipelineReport.Result.Should().Be(Result.Failed);
            _pipelineReport.Details.Should().ContainAll("resharper-inspections-bitbucket-pipe", "4");
        }
    }
}
