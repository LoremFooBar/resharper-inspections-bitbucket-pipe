using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketAnnotationTests
{
    public class When_Creating_Annotations_From_Empty_Report : SpecificationBase
    {
        private IEnumerable<Annotation> _annotations;
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
            _annotations = new AnnotationsCreator().CreateAnnotationsFromIssuesReport(_report);
        }

        [Then]
        public void It_Should_Create_Empty_Enumerable()
        {
            _annotations.Should().BeEmpty();
        }
    }
}
