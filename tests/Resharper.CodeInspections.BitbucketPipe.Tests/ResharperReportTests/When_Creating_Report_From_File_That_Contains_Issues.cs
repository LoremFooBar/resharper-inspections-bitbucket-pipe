using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.ResharperReportTests
{
    public class When_Creating_Report_From_File_That_Contains_Issues : SpecificationBase
    {
        private Report _report;

        protected override async Task WhenAsync()
        {
            _report = await Report.CreateFromFileAsync(TestUtils.GetNonEmptyReportFilePath());
        }

        [Then]
        public void It_Should_Create_Report_Without_Issues()
        {
            _report.HasAnyIssues.Should().BeTrue();
        }

        [Then]
        public void It_Should_Deserialize_Issues_Correctly()
        {
            _report.AllIssues.Should().HaveCount(4);
            _report.IssueTypes.Types.Should().HaveCount(3);
        }
    }
}
