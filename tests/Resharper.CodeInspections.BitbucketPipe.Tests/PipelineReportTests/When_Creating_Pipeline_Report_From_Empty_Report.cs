using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipelineReportTests
{
    public class When_Creating_Pipeline_Report_From_Empty_Report : SpecificationBase
    {
        private PipelineReport _pipelineReport;
        private ReSharperReportCreator _reSharperReportCreator;

        protected override void Given()
        {
            base.Given();


            var environmentInfo = new BitbucketEnvironmentInfo
            {
                Workspace = "workspace",
                RepoSlug = "repo-slug",
                CommitHash = "f46f058a160a42c68e4b30ee4598cbfc"
            };

            var bitbucketClientSimpleMock = new BitbucketClientSimpleMock(true, true, environmentInfo);
            var pipeOptions = new OptionsWrapper<PipeOptions>(new PipeOptions());
            _reSharperReportCreator = new ReSharperReportCreator(pipeOptions, bitbucketClientSimpleMock.BitbucketClient,
                NullLogger<ReSharperReportCreator>.Instance);
        }

        protected override async Task WhenAsync()
        {
            var report = await _reSharperReportCreator.CreateFromFileAsync(TestData.EmptyReportFilePath);
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
