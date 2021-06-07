using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BuildStatusTests
{
    public class When_Creating_Build_Status_From_Empty_Report : SpecificationBase
    {
        private BuildStatus _buildStatus;

        protected override async Task WhenAsync()
        {
            var report = await Report.CreateFromFileAsync(TestUtils.GetEmptyReportFilePath());
            _buildStatus = BuildStatus.CreateFromPipelineReport(PipelineReport.CreateFromIssuesReport(report), "workspace",
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
