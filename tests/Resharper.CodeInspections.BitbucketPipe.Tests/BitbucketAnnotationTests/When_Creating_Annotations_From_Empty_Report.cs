using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketAnnotationTests
{
    public class When_Creating_Annotations_From_Empty_Report : SpecificationBase
    {
        private IEnumerable<Annotation> _annotations;

        protected override async Task WhenAsync()
        {
            var report = await Report.CreateFromFileAsync(TestUtils.GetEmptyReportFilePath());
            _annotations = Annotation.CreateFromIssuesReport(report);
        }

        [Then]
        public void It_Should_Create_Empty_Enumerable()
        {
            _annotations.Should().BeEmpty();
        }
    }
}
