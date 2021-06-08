using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.ResharperReportTests
{
    public class When_Creating_Report_From_File_That_Does_Not_Contain_Issues : SpecificationBase
    {
        private Report _report;

        protected override async Task WhenAsync()
        {
            _report = await Report.CreateFromFileAsync(ExampleReports.GetEmptyReportFilePath());
        }

        [Then]
        public void It_Should_Create_Report_Without_Issues()
        {
            _report.HasAnyIssues.Should().BeFalse();
        }
    }
}
