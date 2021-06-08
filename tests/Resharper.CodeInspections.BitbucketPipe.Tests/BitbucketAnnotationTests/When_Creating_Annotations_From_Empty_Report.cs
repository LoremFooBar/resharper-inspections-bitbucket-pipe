using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketAnnotationTests
{
    public class When_Creating_Annotations_From_Empty_Report : SpecificationBase
    {
        private IEnumerable<Annotation> _annotations;
        private Report _report;

        protected override async Task GivenAsync()
        {
            await base.GivenAsync();
            _report = await Report.CreateFromFileAsync(ExampleReports.GetEmptyReportFilePath());
        }

        protected override void When()
        {
            _annotations = new AnnotationsCreator().CreateAnnotationsFromIssuesReport(_report);
        }

        [Then]
        public void It_Should_Create_Empty_Enumerable()
        {
            _annotations.Should().BeEmpty();
        }
    }
}
