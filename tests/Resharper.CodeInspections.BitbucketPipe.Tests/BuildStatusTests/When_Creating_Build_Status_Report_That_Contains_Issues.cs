using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BuildStatusTests
{
    public class When_Creating_Build_Status_Report_That_Contains_Issues : SpecificationBase
    {
        private BuildStatus _buildStatus;

        protected override async Task WhenAsync()
        {
            var report = await Report.CreateFromFileAsync(TestUtils.GetNonEmptyReportFilePath());
            _buildStatus = BuildStatus.CreateFromPipelineReport(PipelineReport.CreateFromIssuesReport(report), "workspace",
                "repoSlug");
        }

        [Then]
        public void It_Should_Have_Failed_State()
        {
            _buildStatus.State.Should().Be(State.Failed);
        }

        [Then]
        public void It_Should_Indicate_Number_Of_Issues_In_Description()
        {
            _buildStatus.Description.ToLowerInvariant().Should().Contain("4");
        }
    }
}
