using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipelineReportTests
{
    public class When_Creating_Pipeline_Report_From_Empty_Report : SpecificationBase
    {
        private PipelineReport _pipelineReport;

        protected override async Task WhenAsync()
        {
            var report = await Report.CreateFromFileAsync(TestUtils.GetEmptyReportFilePath());
            _pipelineReport = PipelineReport.CreateFromIssuesReport(report);
        }

        [Then]
        public void It_Should_Create_Pipeline_Report_Without_Issues()
        {
            _pipelineReport.TotalIssues.Should().Be(0);
            _pipelineReport.Result.Should().Be(Result.Passed);
            _pipelineReport.Details.Should().NotBeNullOrEmpty().And
                .Subject.ToLowerInvariant().Should().ContainAll("dotnet-coverage-report-bitbucket-pipe", "no issue");
        }
    }
}
