using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.ReportCreatorTests
{
    public class When_Creating_Report_And_Requesting_To_Include_Only_Issues_In_Diff : SpecificationBase
    {
        private SimpleReport _report;
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

            var messageHandler = new Mock<HttpMessageHandler>();

            var httpClient = messageHandler.CreateClient();
            var bitbucketClient = new BitbucketClient(httpClient,
                new OptionsWrapper<BitbucketAuthenticationOptions>(new BitbucketAuthenticationOptions()),
                new OptionsWrapper<PipeOptions>(new PipeOptions()),
                environmentInfo,
                NullLogger<BitbucketClient>.Instance);

            messageHandler
                .SetupRequest($"{httpClient.BaseAddress}diff/{environmentInfo.CommitHash}")
                .ReturnsResponse(HttpStatusCode.OK, TestData.DiffText, "text/plain");

            var pipeOptions = Mock.Of<IOptions<PipeOptions>>(options => options.Value.IncludeOnlyIssuesInDiff == true);
            _reSharperReportCreator = new ReSharperReportCreator(pipeOptions, bitbucketClient,
                NullLogger<ReSharperReportCreator>.Instance);
        }

        protected override async Task WhenAsync()
        {
            _report = await _reSharperReportCreator.CreateFromFileAsync(TestData.NonEmptyReportFilePath);
        }

        [Then]
        public void It_Should_Create_Report_With_Issues()
        {
            _report.HasAnyIssues.Should().BeTrue();
        }

        [Then]
        public void It_Should_Set_Filtered_Issues_Based_On_Diff()
        {
            _report.Issues.Should()
                .NotBeNull()
                .And.HaveCount(3)
                .And.OnlyContain(issue => issue.Line >=1 && issue.Line <= 6);
        }
    }
}
