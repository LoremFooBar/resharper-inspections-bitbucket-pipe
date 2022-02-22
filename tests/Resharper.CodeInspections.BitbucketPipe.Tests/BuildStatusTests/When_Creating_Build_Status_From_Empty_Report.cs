using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BuildStatusTests
{
    public class When_Creating_Build_Status_From_Empty_Report : SpecificationBase
    {
        private BuildStatus _buildStatus;
        private SimpleReport _report;

        protected override async Task GivenAsync()
        {
            await base.GivenAsync();

            var environmentInfo = new BitbucketEnvironmentInfo
            {
                Workspace = "workspace",
                RepoSlug = "repo-slug",
                CommitHash = "f46f058a160a42c68e4b30ee4598cbfc"
            };

            var bitbucketClientSimpleMock = new BitbucketClientSimpleMock(true, true, environmentInfo);
            var pipeOptions = new OptionsWrapper<PipeOptions>(new PipeOptions());
            var reSharperReportCreator = new ReSharperReportCreator(pipeOptions, bitbucketClientSimpleMock.BitbucketClient,
                NullLogger<ReSharperReportCreator>.Instance);

            _report = await reSharperReportCreator.CreateFromFileAsync(TestData.EmptyReportFilePath);
        }

        protected override void When()
        {
            _buildStatus = BuildStatus.CreateFromPipelineReport(PipelineReport.CreateFromIssuesReport(_report), "workspace",
                "repoSlug");
        }

        [Then]
        public void It_Should_Have_Successful_State()
        {
            _buildStatus.State.Should().Be(State.Successful);
        }

        [Then]
        public void It_Should_Have_Correct_Description()
        {
            _buildStatus.Description.ToLowerInvariant().Should().Contain("no issues");
        }
    }
}
