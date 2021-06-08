using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketAnnotationTests
{
    public class When_Creating_Annotations_From_Report_That_Contains_Issues : SpecificationBase
    {
        private List<Annotation> _annotations;
        private Report _report;

        protected override async Task GivenAsync()
        {
            await base.GivenAsync();
            _report = await Report.CreateFromFileAsync(ExampleReports.GetNonEmptyReportFilePath());
        }

        protected override void When()
        {
            _annotations = new AnnotationsCreator().CreateAnnotationsFromIssuesReport(_report).ToList();
        }

        [Then]
        public void It_Should_Create_Annotations_Based_On_Issues_In_Report()
        {
            _annotations.Should()
                .HaveCount(4).And
                .OnlyContain(annotation => annotation.AnnotationType == AnnotationType.CodeSmell);

            _annotations[0].Line.Should().Be(0);
            _annotations[1].Line.Should().Be(2);
            _annotations[2].Line.Should().Be(5);
            _annotations[3].Line.Should().Be(7);

            _annotations[0].Summary.Should().Be("Redundant using directive");
            _annotations[1].Summary.Should().Be("Redundant using directive");
            _annotations[2].Summary.Should().Be("Namespace does not correspond to file location");
            _annotations[3].Summary.Should().Be("Type is never used: Non-private accessibility");

            _annotations[0].Details.Should()
                .ContainAll("Using directive is not required by the code and can be safely removed",
                    "Wiki URL: https://www.jetbrains.com/resharperplatform/help?Keyword=RedundantUsingDirective");
            _annotations[1].Details.Should()
                .ContainAll("Using directive is not required by the code and can be safely removed",
                    "Wiki URL: https://www.jetbrains.com/resharperplatform/help?Keyword=RedundantUsingDirective");
            _annotations[2].Details.Should()
                .ContainAll(
                    "Namespace does not correspond to file location, should be: 'Resharper.CodeInspections.BitbucketPipe.Tests.ResharperReportTests'",
                    "Wiki URL: https://www.jetbrains.com/resharperplatform/help?Keyword=CheckNamespace");
            _annotations[3].Details.Should()
                .ContainAll("Class 'When_Creating_Annotations_From_Empty_Report' is never used",
                    "Wiki URL: https://www.jetbrains.com/resharperplatform/help?Keyword=UnusedType.Global");

            _annotations[0].Path.Should()
                .Be(
                    @"tests/Resharper.CodeInspections.BitbucketPipe.Tests/ResharperReportTests/When_Creating_Report_From_File_That_Does_Not_Contain_Issues.cs");
            _annotations[1].Path.Should()
                .Be(
                    @"tests/Resharper.CodeInspections.BitbucketPipe.Tests/ResharperReportTests/When_Creating_Report_From_File_That_Does_Not_Contain_Issues.cs");
            _annotations[2].Path.Should()
                .Be(
                    @"tests/Resharper.CodeInspections.BitbucketPipe.Tests/ResharperReportTests/When_Creating_Report_From_File_That_Does_Not_Contain_Issues.cs");
            _annotations[3].Path.Should()
                .Be(
                    @"tests/Resharper.CodeInspections.BitbucketPipe.Tests/ResharperReportTests/When_Creating_Report_From_File_That_Does_Not_Contain_Issues.cs");

            _annotations.Select(annotation => annotation.ExternalId).Should().OnlyHaveUniqueItems();
        }
    }
}
